using System;
using UnityEngine;

/// <summary>
/// 유저가 조종하는 플레이어 클래스
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

    [Space(10f), Header("애니메이션 클립")]
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

    [Space(10f), Header("체력")]
    //활성 체력'으로, 남아있는 체력을 의미한다.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _remainLife;

    public byte remainLife {
        get
        {
            return _remainLife;
        }
    }

    //회색 체력은 '상처'로, 일반적인 회복제로 회복할 수 있는 체력이다.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _lossLife;

    public byte lossLife {
        get
        {
            return _lossLife;
        }
    }
    //최대치의 체력을 의미한다.
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

    [Space(10f), Header("마력")]
    //'활성 마력'으로, 남아있는 마력을 의미한다.
    [SerializeField, Range(0, byte.MaxValue)]
    private byte _remainMana;

    public byte remainMana {
        get
        {
            return _remainMana;
        }
    }
    //최대치의 마력을 의미한다.
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
        //직진
        if ((Vector2)getTransform.rotation.eulerAngles == rotation)
        {
            PlayMove(true);
        }
        //유턴
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
        //피 채우는 용도라면
        if (result > 0)
        {
            //최대 체력 보다 값이 넘을 경우는 최대 체력이 된다.
            if (_remainLife + result > _maxLife - _lossLife)
            {
                _remainLife = (byte)(_maxLife - _lossLife);
            }
            //그렇지 않으면 값을 더해준다.
            else
            {
                _remainLife += (byte)result;
            }
        }
        //피 깎는 용도라면
        else
        {
            //
            if(-result < _remainLife)
            {
                _remainLife -= (byte)-result;
            }
            //사망
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