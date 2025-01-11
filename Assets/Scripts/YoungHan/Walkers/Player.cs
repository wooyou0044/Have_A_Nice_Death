using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// ������ �����ϴ� �÷��̾� Ŭ����
/// </summary>
[RequireComponent(typeof(Animator))]
public sealed class Player : Runner, IHittable
{
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

    //�ǰ� �׼� ��������Ʈ
    private Action<IHittable, int> _hitAction = null;
    //��ų ��� �׼� ��������Ʈ

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
        //    //�����ϸ� �ִϸ��̼� �ٲٱ�
        //}
    }
    
    public void MoveDown()
    {

    }

    public void Attack1()
    {
        if(isAlive == true)
        {
            Debug.Log("����");
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

    }

    public Collider2D GetCollider2D()
    {
        return getCollider2D;
    }
}