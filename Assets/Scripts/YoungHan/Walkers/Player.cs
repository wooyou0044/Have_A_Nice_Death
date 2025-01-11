using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 유저가 조종하는 플레이어 클래스
/// </summary>
[RequireComponent(typeof(Animator))]
public sealed class Player : Runner, IHittable
{
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
    private AnimationClip _comboMove1Clip = null;
    [SerializeField]
    private AnimationClip _comboMove2Clip = null;
    [SerializeField]
    private AnimationClip _comboMove3Clip = null;
    [SerializeField]
    private AnimationClip _comboMove4Clip = null;

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

    private bool _hasSpriteRenderer = false;

    private SpriteRenderer _spriteRenderer = null;

    private SpriteRenderer getSpriteRenderer {
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

    //이건 무기 클래스가 대신해야 한다
    private enum Combo: byte
    {
        None,
        Combo1,
        Combo2,
        Combo3,
        Combo4,
    }

    [SerializeField]
    private Combo _comboCount = Combo.None;

    //피격 액션 델리게이트
    private Action<IHittable, int> _hitAction = null;
    //스킬 사용 액션 델리게이트

    //private Func<Interaction, bool> _interactionFunction = null;

    private IEnumerator _animationCoroutine = null;

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
            if (flip == true)
            {
                getSpriteRenderer.flipX = true;
            }
            if (first != null)
            {
                getAnimator.Play(first.name);
                yield return null;
                yield return new WaitWhile(() => IsPlaying(first.name));
            }
            if (flip == true)
            {
                getSpriteRenderer.flipX = false;
            }
            if (second != null)
            {
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

    private AnimationClip GetCurrentClips()
    {
        AnimatorClipInfo[] clipInfos = getAnimator.GetCurrentAnimatorClipInfo(0);
        int length = clipInfos != null ? clipInfos.Length : 0;
        if(length > 0)
        {
            return clipInfos[0].clip;
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
        base.OnCollisionEnter2D(collision);
        if (isGrounded == true)
        {
            getSpriteRenderer.flipX = false;
            if (GetCurrentClips() != _dashClip)
            {
                Play(_jumpLandingClip, _idleClip, false);
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);
        if (isGrounded == true && getRigidbody2D.velocity.y == 0 && GetCurrentClips() == _jumpFallingClip)
        {
            Play(_jumpLandingClip, _idleClip, false);
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionExit2D(collision);
        if(isGrounded == false)
        {
            getSpriteRenderer.flipX = false;
            Play(null, _jumpFallingClip, false);
        }
    }

    public override void MoveLeft()
    {
        if (isAlive == true)
        {
            base.MoveLeft();
            PlayMove(ITransformable.LeftRotation);
        }
    }

    public override void MoveRight()
    {
        if (isAlive == true)
        {
            base.MoveRight();
            PlayMove(ITransformable.RightRotation);
        }
    }

    public override void MoveStop()
    {
        if (isAlive == true)
        {
            AnimationClip animationClip = GetCurrentClips();
            if(animationClip == _runClip)
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
            float velocity = getRigidbody2D.velocity.y;
            base.Jump();
            if(velocity != getRigidbody2D.velocity.y)
            {
                _comboCount = Combo.None;
                getSpriteRenderer.flipX = false;
                Play(_jumpStartClip, _jumpFallingClip , false);
            }
        }
    }

    public override void Dash()
    {
        if (isAlive == true)
        {
            float velocity = getRigidbody2D.velocity.x;
            base.Dash();
            if (velocity != getRigidbody2D.velocity.x)
            {
                _comboCount = Combo.None;
                getSpriteRenderer.flipX = false;
                Play(_dashClip, _jumpFallingClip, false);
            }
        }
    }

    public void Initialize(Action<IHittable, int> hit)
    {
        _hitAction = hit;
        //_strikeAction = strike;        //_interactionFunction = interaction;
    }
    public void MoveUp()
    {
        //if(_interactionFunction != null && _interactionFunction.Invoke(Interaction.MoveUp) == true)
        //{
        //    //성공하면 애니메이션 바꾸기
        //}
    }
    
    public void MoveDown()
    {

    }

    public void Attack1()
    {
        if(isAlive == true)
        {
            if (IsLevitate() == false && _comboCount < Combo.Combo4)
            {
                switch(_comboCount)
                {
                    case Combo.None:
                        Levitate(false);
                        Play(_comboMove1Clip, isGrounded == true? _idleClip : _jumpFallingClip, false);
                        break;
                    case Combo.Combo1:
                        Levitate(false);
                        Play(_comboMove2Clip, isGrounded == true ? _idleClip : _jumpFallingClip, false);
                        break;
                    case Combo.Combo2:
                        Levitate(false);
                        Play(_comboMove3Clip, isGrounded == true ? _idleClip : _jumpFallingClip, false);
                        break;
                    case Combo.Combo3:
                        Levitate(false);
                        Play(_comboMove4Clip, isGrounded == true ? _idleClip : _jumpFallingClip, false);
                        break;
                }
                _comboCount++;
            }
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