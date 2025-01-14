using System;
using UnityEngine;

/// <summary>
/// ������ �����ϴ� �÷��̾� Ŭ����
/// </summary>
[RequireComponent(typeof(AnimatorPlayer))]
public sealed class Player : Runner, IHittable
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

    private float _chargingTime = 0;

    [SerializeField]
    private Weapon _weapon1;

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
            getAnimatorPlayer.Play(_jumpFallingClip, false);
        }
    }

    public override void MoveLeft()
    {
        if (isAlive == true)
        {
            RigidbodyConstraints2D rigidbodyConstraints2D = getRigidbody2D.constraints;
            if (rigidbodyConstraints2D != RigidbodyConstraints2D.FreezePositionX &&
                rigidbodyConstraints2D != RigidbodyConstraints2D.FreezePositionY &&
                rigidbodyConstraints2D != RigidbodyConstraints2D.FreezePosition)
            {
                base.MoveLeft();
                PlayMove(LeftRotation);
            }
        }
    }

    public override void MoveRight()
    {
        if (isAlive == true)
        {
            RigidbodyConstraints2D rigidbodyConstraints2D = getRigidbody2D.constraints;
            if (rigidbodyConstraints2D != RigidbodyConstraints2D.FreezePositionX &&
                rigidbodyConstraints2D != RigidbodyConstraints2D.FreezePositionY &&
                rigidbodyConstraints2D != RigidbodyConstraints2D.FreezePosition)
            {
                base.MoveRight();
                PlayMove(RightRotation);
            }
        }
    }

    public override void MoveStop()
    {
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
            _boundingFunction?.Invoke(false);
            float velocity = getRigidbody2D.velocity.y;
            base.Jump();
            if(velocity != getRigidbody2D.velocity.y)
            {
                getAnimatorPlayer.Play(_jumpStartClip, _jumpFallingClip , false);
            }
        }
    }

    public override void Dash()
    {
        if (isAlive == true)
        {
            base.Dash();
            RigidbodyConstraints2D rigidbodyConstraints2D = getRigidbody2D.constraints;
            if (rigidbodyConstraints2D == (RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation))
            {
                _boundingFunction?.Invoke(false);
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
    }

    public void Initialize(Action<IHittable, int> hit, Action<Strike, Strike.Area, GameObject> strike, Action<GameObject, Vector2, Transform> effect, Func<bool, bool> bounding, Func<Projectile, Projectile> projectile)
    {
        _hitAction = hit;
        _strikeAction = strike;
        _effectAction = effect;
        _boundingFunction = bounding;
        _projectileFunction = projectile;
    }

    public void MoveUp()
    {
        if (_boundingFunction != null && _boundingFunction.Invoke(true) == true)
        {
            getAnimatorPlayer.Play(_zipUpClip);
        }
    }
    
    public void MoveDown()
    {

    }

    public void AttackBasic(bool pressed)
    {
        if(_weapon1 != null)
        {
            if(pressed == true)
            {

            }
            else
            {

            }
        }
        if(pressed == false)
        {
            _chargingTime = 0;
        }
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