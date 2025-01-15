using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// ������ �����ϴ� �÷��̾� Ŭ����
/// </summary>
[RequireComponent(typeof(AnimatorPlayer))]
[RequireComponent(typeof(WeaponSet))]
public sealed partial class Player : Runner, IHittable
{
    private static readonly float MinimumDropVelocity = -0.49f;

    private bool _hasAnimatorPlayer = false;

    private AnimatorPlayer _animatorPlayer = null;

    private AnimatorPlayer getAnimatorPlayer
    {
        get
        {
            if (_hasAnimatorPlayer == false)
            {
                _hasAnimatorPlayer = true;
                _animatorPlayer = GetComponent<AnimatorPlayer>();
            }
            return _animatorPlayer;
        }
    }

    private bool _hasWeaponSet = false;

    private WeaponSet _weaponSet = null;

    private WeaponSet getWeaponSet
    {
        get
        {
            if(_hasWeaponSet == false)
            {
                _hasWeaponSet = true;
                _weaponSet = GetComponent<WeaponSet>();
            }
            return _weaponSet;
        }
    }

    [Space(10f), Header("�ִϸ��̼� Ŭ��")]
    [SerializeField]
    private AnimationClip _idleClip = null;
    [SerializeField]
    private AnimationClip _idleToRunClip = null;
    [SerializeField]
    private AnimationClip _idleUturnClip = null;
    [SerializeField]
    private AnimationClip _runClip = null;
    [SerializeField]
    private AnimationClip _runToIdleClip = null;
    [SerializeField]
    private AnimationClip _runUturnClip = null;
    [SerializeField]
    private AnimationClip _jumpStartClip = null;
    [SerializeField]
    private AnimationClip _jumpFallingClip = null;
    [SerializeField]
    private AnimationClip _jumpLandingClip = null;
    [SerializeField]
    private AnimationClip _dashClip = null;
    [SerializeField]
    private AnimationClip _zipUpClip = null;

    public enum Direction
    {
        Center,
        Up,
        Down
    }

    [SerializeField]
    private Direction _direction = Direction.Center;

    [Space(10f), Header("ü��")]
    //Ȱ�� ü��'����, �����ִ� ü���� �ǹ��Ѵ�.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _remainLife;

    public byte remainLife {
        get
        {
            return _remainLife;
        }
    }

    //ȸ�� ü���� '��ó'��, �Ϲ����� ȸ������ ȸ���� �� �ִ� ü���̴�.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _lossLife;

    public byte lossLife {
        get
        {
            return _lossLife;
        }
    }
    //�ִ�ġ�� ü���� �ǹ��Ѵ�.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _maxLife;

    public byte maxLife {
        get
        {
            return _maxLife;
        }
    }

    public bool isAlive
    {
        get
        {
            return _remainLife > 0 || _maxLife <= _remainLife;
        }
    }

    [Space(10f), Header("����")]
    //'Ȱ�� ����'����, �����ִ� ������ �ǹ��Ѵ�.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _remainMana;

    public byte remainMana {
        get
        {
            return _remainMana;
        }
    }

    //�ִ�ġ�� ������ �ǹ��Ѵ�.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _maxMana;

    public byte maxMana {
        get
        {
            return _maxMana;
        }
    }

    [SerializeField]
    private float _levitateTime = 0.5f;


    private IEnumerator _coroutine = null;

    private Action _escapeAction = null;
    private Action<IHittable, int> _hitAction = null;
    private Action<Strike, Strike.Area, GameObject> _strikeAction = null;
    private Action<GameObject, Vector2, Transform> _effectAction = null;
    private Func<bool, bool> _boundingFunction = null;
    private Func<Projectile, Projectile> _projectileFunction = null;

    private void PlayMove(bool straight)
    {
        AnimationClip animationClip = getAnimatorPlayer.GetCurrentClips();
        if (straight == false)
        {
            if (animationClip == _idleClip)
            {
                getAnimatorPlayer.Play(_idleUturnClip, _idleClip, true);
            }
            else if (animationClip == _runToIdleClip || animationClip == _idleToRunClip || animationClip == _idleUturnClip || animationClip == _runClip)
            {
                getAnimatorPlayer.Play(_runUturnClip, _runClip, true);
            }
        }
        else if (animationClip == _idleClip)
        {
            getAnimatorPlayer.Play(_idleToRunClip, _runClip, false);
        }
    }

    private void PlayMove(Vector2 rotation)
    {
        //����
        if ((Vector2)getTransform.rotation.eulerAngles == rotation)
        {
            PlayMove(true);
        }
        //����
        else
        {
            PlayMove(false);
            getTransform.rotation = Quaternion.Euler(rotation);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (_lossLife > _maxLife)
        {
            _lossLife = _maxLife;
        }
        if(_remainLife > _maxLife - _lossLife)
        {
            _remainLife = (byte)(_maxLife - _lossLife);
        }
        if (_remainMana > _maxMana)
        {
            _remainMana = _maxMana;
        }
    }
#endif

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        bool isGrounded = this.isGrounded;
        base.OnCollisionEnter2D(collision);
        if (isGrounded != this.isGrounded)
        {
            getAnimatorPlayer.Play(_jumpLandingClip, _idleClip, false);
            _escapeAction?.Invoke();
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);
        if (isGrounded == false && getRigidbody2D.velocity.y < MinimumDropVelocity)
        {
            getAnimatorPlayer.Play(_jumpFallingClip);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(getAnimatorPlayer.IsPlaying(_zipUpClip) == true)
        {
            getAnimatorPlayer.Play(_jumpFallingClip);
        }
    }

    private bool CanMoveState()
    {
        RigidbodyConstraints2D rigidbodyConstraints2D = getRigidbody2D.constraints;
        if (rigidbodyConstraints2D != RigidbodyConstraints2D.FreezePositionX &&
              rigidbodyConstraints2D != RigidbodyConstraints2D.FreezePositionY &&
              rigidbodyConstraints2D != RigidbodyConstraints2D.FreezePosition)
        {
            AnimationClip animationClip = getAnimatorPlayer.GetCurrentClips();
            if (animationClip == _jumpFallingClip && rigidbodyConstraints2D == (RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation))
            {
                return false;
            }
            return animationClip != _dashClip && animationClip != _zipUpClip;
        }
        return false;
    }

    public override void MoveLeft()
    {
        if (isAlive == true && CanMoveState() == true)
        {
            base.MoveLeft();
            PlayMove(LeftRotation);
        }
    }

    public override void MoveRight()
    {
        if (isAlive == true && CanMoveState() == true)
        {
            base.MoveRight();
            PlayMove(RightRotation);
        }
    }

    public override void MoveStop()
    {
        _direction = Direction.Center;
        if (isAlive == true)
        {
            if(getAnimatorPlayer.IsPlaying(_runClip) == true)
            {
                getAnimatorPlayer.Play(_runToIdleClip, _idleClip, false);
            }
            base.MoveStop();
        }
    }

    public override void Jump()
    {
        if (isAlive == true)
        {
            float velocity = getRigidbody2D.velocity.y;
            base.Jump();
            if(velocity != getRigidbody2D.velocity.y || getAnimatorPlayer.IsPlaying(_zipUpClip) == true)
            {
                _escapeAction?.Invoke();
                getAnimatorPlayer.Play(_jumpStartClip, _jumpFallingClip , false);
            }
        }
    }

    public override void Dash(Vector2 direction)
    {
        if (isAlive == true && CanDash() == true)
        {
            if(direction.normalized.x < 0)
            {
                PlayMove(LeftRotation);
            }
            else
            {
                PlayMove(RightRotation);
            }
            base.Dash(direction);
            _escapeAction?.Invoke();
            if (isGrounded == true)
            {
                getAnimatorPlayer.Play(_dashClip, _idleClip, false);
            }
            else
            {
                getAnimatorPlayer.Play(_dashClip, _jumpFallingClip, false);
            }
        }
    }

    public void Initialize(Action escape, Action<IHittable, int> hit, Action<GameObject, Vector2, Transform> effect, Action<Strike, Strike.Area, GameObject> strike, Func<bool, bool> bounding, Func<Projectile, Projectile> projectile)
    {
        _escapeAction = escape;
        _hitAction = hit;
        _strikeAction = strike;
        _effectAction = effect;
        _boundingFunction = bounding;
        _projectileFunction = projectile;
    }

    public void MoveUp()
    {
        if (isAlive == true)
        {
            _direction = Direction.Up;
            AnimationClip clip = getAnimatorPlayer.GetCurrentClips();
            if (clip != _zipUpClip && clip != _dashClip && _boundingFunction != null && _boundingFunction.Invoke(true) == true)
            {
                getAnimatorPlayer.Play(_zipUpClip);
                RecoverJumpCount();
            }
        }
    }
    
    public void MoveDown()
    {
        if (isAlive == true)
        {
            _direction = Direction.Down;
            if (getAnimatorPlayer.IsPlaying(_zipUpClip) == true && _boundingFunction != null && _boundingFunction.Invoke(false) == true)
            {
                getAnimatorPlayer.Play(_jumpFallingClip);
            }
        }
    }

    public void AttackScythe(bool pressed)
    {
        if(isAlive == true && getWeaponSet.TryScythe(pressed, _direction, _effectAction, _strikeAction, _projectileFunction, getAnimatorPlayer.animator) == true)
        {
            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = DoPlay();
            StartCoroutine(_coroutine);
            IEnumerator DoPlay()
            {
                getAnimatorPlayer.Stop();
                Levitate(true, _levitateTime);
                yield return new WaitForSeconds(_levitateTime);
                if (isGrounded == true)
                {
                    getAnimatorPlayer.Play(_idleClip);
                }
                else
                {
                    getAnimatorPlayer.Play(_jumpFallingClip);
                }
                _coroutine = null;
            }
        }
        getWeaponSet.Recharge(pressed);
    }

    public void Attack1()
    {

    }

    public void Attack2()
    {

    }

    public void AttackWide()
    {

    }

    public void Interact()
    {

    }

    public void Heal(bool anima = false)
    {

    }

    public void Hit(Strike strike)
    {
        int result = strike.result;
        //�� ä��� �뵵���
        if (result > 0)
        {
            //�ִ� ü�� ���� ���� ���� ���� �ִ� ü���� �ȴ�.
            if (_remainLife + result > _maxLife - _lossLife)
            {
                _remainLife = (byte)(_maxLife - _lossLife);
            }
            //�׷��� ������ ���� �����ش�.
            else
            {
                _remainLife += (byte)result;
            }
        }
        //�� ��� �뵵���
        else
        {
            //
            if(-result < _remainLife)
            {
                _remainLife -= (byte)-result;
            }
            //���
            else
            {
                _remainLife = 0;
            }
        }
        _hitAction?.Invoke(this, result);
    }

    public Collider2D GetCollider2D()
    {
        return getCollider2D;
    }
}