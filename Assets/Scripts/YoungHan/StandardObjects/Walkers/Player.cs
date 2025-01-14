using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 유저가 조종하는 플레이어 클래스
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public sealed class Player : Runner, IHittable
{

    private bool _hasSpriteRenderer = false;

    private SpriteRenderer _spriteRenderer = null;

    private SpriteRenderer getSpriteRenderer
    {
        get
        {
            if (_hasSpriteRenderer == false)
            {
                _hasSpriteRenderer = true;
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            return _spriteRenderer;
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

    private bool _hasAnimator = false;

    private Animator _animator = null;

    private Animator getAnimator
    {
        get
        {
            if (_hasAnimator == false)
            {
                _hasAnimator = true;
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }


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

    [SerializeField]
    private Weapon _weapon1;

    private Action<IHittable, int> _hitAction = null;

    private Action<Strike, Strike.Area, GameObject> _strikeAction = null;

    private Action<GameObject, Vector2, Transform> _effectAction = null;

    private Func<bool, bool> _boundingFunction = null;

    private Func<Projectile, Projectile> _projectileFunction = null;

    private IEnumerator _animationCoroutine = null;

    /// <summary>
    /// 첫 번째 애니메이션을 재생시키고 두 번째 애니메이션을 재생시키는 함수, bool은 스프라이트 렌더러를 일시적으로 반전 시킴
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <param name="flip"></param>
    private void Play(AnimationClip first, AnimationClip second, bool flip)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        _animationCoroutine = DoPlay();
        StartCoroutine(_animationCoroutine);
        IEnumerator DoPlay()
        {  
            if (first != null)
            {
                if (flip == true)
                {
                    getSpriteRenderer.flipX = true;
                }
                getAnimator.Play(first.name);
                yield return null;
                yield return new WaitWhile(() => IsPlaying(first.name));
            }
            if (second != null)
            {
                if (flip == true)
                {
                    getSpriteRenderer.flipX = false;
                }
                getAnimator.Play(second.name);
                yield return null;
                yield return new WaitWhile(() => IsPlaying(second.name));
            }
            _animationCoroutine = null;
        }
    }

    private void PlayMove(bool straight)
    {
        AnimationClip animationClip = GetCurrentClips();
        if (straight == false)
        {
            if (animationClip == _idleClip)
            {
                Play(_idleUturnClip, _idleClip, true);
            }
            else if (animationClip == _runToIdleClip || animationClip == _idleToRunClip || animationClip == _idleUturnClip || animationClip == _runClip)
            {
                Play(_runUturnClip, _runClip, true);
            }
        }
        else if (animationClip == _idleClip)
        {
            Play(_idleToRunClip, _runClip, false);
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

    private bool IsPlaying(string animation)
    {
        AnimatorStateInfo stateInfo = getAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime < 1.0f && stateInfo.IsName(animation) == true;
    }

    private IEnumerator DoRecoverAnimation()
    {
        Levitate(false);
        yield return null;
        do
        {
            yield return null;
        } while (getAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0);
        if(isGrounded == true)
        {
            Play(null, _idleClip, false);
        }
        else
        {
            Play(null, _jumpFallingClip, false);
        }
        _animationCoroutine = null;
    }

    private AnimationClip GetCurrentClips()
    {
        if (gameObject.activeInHierarchy == true)
        {
            AnimatorClipInfo[] clipInfos = getAnimator.GetCurrentAnimatorClipInfo(0);
            int length = clipInfos != null ? clipInfos.Length : 0;
            if (length > 0)
            {
                return clipInfos[0].clip;
            }
        }
        return null;
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
            Debug.Log(this.isGrounded);
            getSpriteRenderer.flipX = false;
            Play(_jumpLandingClip, _idleClip, false);
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        bool isGrounded = this.isGrounded;
        base.OnCollisionStay2D(collision);
        if (isGrounded != this.isGrounded /*&& getRigidbody2D.velocity.y == 0*/)
        {
            //Debug.Log(this.isGrounded);
            //getSpriteRenderer.flipX = false;
            //Play(_jumpLandingClip, _idleClip, false);
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        bool isGrounded = this.isGrounded;
        base.OnCollisionExit2D(collision);
        if (isGrounded != this.isGrounded)
        {
            //Debug.Log(this.isGrounded);
            //getSpriteRenderer.flipX = false;
            //Play(null, _jumpFallingClip, false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(GetCurrentClips() == _zipUpClip)
        {
            Play(null, _jumpFallingClip, false);
        }
    }

    public override void MoveLeft()
    {
        if (isAlive == true)
        {
            AnimationClip animationClip = GetCurrentClips();
            if (animationClip != _dashClip && animationClip != _zipUpClip)
            {
                base.MoveLeft();
                PlayMove(ITransformable.LeftRotation);
            }
        }
    }

    public override void MoveRight()
    {
        if (isAlive == true)
        {
            AnimationClip animationClip = GetCurrentClips();
            if (animationClip != _dashClip && animationClip != _zipUpClip)
            {
                base.MoveRight();
                PlayMove(ITransformable.RightRotation);
            }
        }
    }

    public override void MoveStop()
    {
        if (isAlive == true)
        {
            if(GetCurrentClips() == _runClip)
            {
                Play(_runToIdleClip, _idleClip, false);
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
                getSpriteRenderer.flipX = false;
                Play(_jumpStartClip, _jumpFallingClip , false);
            }
        }
    }

    public override void Dash()
    {
        if (isAlive == true && GetCurrentClips() != _zipUpClip)
        {
            float velocity = getRigidbody2D.velocity.x;
            base.Dash();
            if (velocity != getRigidbody2D.velocity.x)
            {
                getSpriteRenderer.flipX = false;
                Play(_dashClip, _jumpFallingClip, false);
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
        //if(_boundingFunction != null && _boundingFunction.Invoke(true) == true)
        //{
        //    Play(null, _zipUpClip, false);
        //}
    }
    
    public void MoveDown()
    {

    }

    public void Attack1()
    {
        if(isAlive == true && _weapon1 != null && _weapon1.TryUse(getAnimator, getTransform, null, _strikeAction, _effectAction, _projectileFunction) == true)
        {
            //if (_animationCoroutine != null)
            //{
            //    StopCoroutine(_animationCoroutine);
            //}
            //_animationCoroutine = DoRecoverAnimation();
            //StartCoroutine(_animationCoroutine);
        }
    }

    public void Attack2()
    {

    }

    public void Attack3()
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
        if(result > 0)
        {
            //피 채우는 용도
        }
        else
        {
            //피 깎는 용도
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