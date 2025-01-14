using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// <summary>
/// ���ϴ� �ִϸ��̼��� ���۽�ų �� �ִ� Ŭ����
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class AnimatorPlayer : MonoBehaviour
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

    private bool _hasAnimator = false;

    private Animator _animator = null;

    public Animator animator
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

    private IEnumerator _coroutine = null;

    /// <summary>
    /// �ִϸ��̼� ������ �� �������� Ȯ���ϴ� ������Ƽ
    /// </summary>
    public bool isEndofFrame
    {
        get
        {
            return _coroutine == null;
        }
    }

    private void OnDisable()
    {
        Stop();
    }

    private void OnApplicationQuit()
    {
        Stop();
    }

    /// <summary>
    /// ������ �ִϸ��̼��� ���߰� ����� �Լ�
    /// </summary>
    public void Stop()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    /// <summary>
    /// Ư�� �ִϸ��̼� �ڵ鷯�� ��� ��Ű�� �Լ�
    /// </summary>
    /// <param name="animatorHandler"></param>
    public void Play(AnimatorHandler animatorHandler)
    {
        if (enabled == false || Application.isPlaying == false)
        {
            return;
        }
        if (animatorHandler != null)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            animatorHandler.Play(animator);
            _coroutine = DoPlay();
            StartCoroutine(_coroutine);
            IEnumerator DoPlay()
            {
                AnimationClip animationClip = GetCurrentClips();
                string name = animationClip != null ? animationClip.name : null;
                yield return null;
                Func<bool> func = () =>
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    return stateInfo.normalizedTime < 1.0f && stateInfo.IsName(name) == true;
                };
                yield return new WaitWhile(func);
                _coroutine = null;
            }
        }
    }

    /// <summary>
    /// Ư�� �ִϸ��̼��� ��� ��Ű�� �Լ�
    /// </summary>
    /// <param name="animationClip">����� �ִϸ��̼� Ŭ��</param>
    /// <param name="force">true�� ��� ������ ���� ���� �ִϸ��̼� ����� ����ϰ� ���Ӱ� ���</param>
    public void Play(AnimationClip animationClip, bool force = true)
    {
        Play(animationClip, null, false, force);
    }

    /// <summary>
    /// Ư�� �ִϸ��̼� �� ���� ���������� ��� ��Ű�� �Լ�
    /// </summary>
    /// <param name="first">����� ù ��° �ִϸ��̼�</param>
    /// <param name="second">����� �� ��° �ִϸ��̼�</param>
    /// <param name="flip">��������Ʈ �������� �Ͻ������� ���� ���״ٰ� ������� �ǵ���</param>
    /// <param name="force">true�� ��� ������ ���� ���� �ִϸ��̼� ����� ����ϰ� ���Ӱ� ���</param>
    public void Play(AnimationClip first, AnimationClip second, bool flip, bool force = true)
    {
        if (enabled == false || Application.isPlaying == false)
        {
            return;
        }
        if (_coroutine != null)
        {
            if (force == false)
            {
                return;
            }
            StopCoroutine(_coroutine);
        }
        _coroutine = DoPlay();
        StartCoroutine(_coroutine);
        IEnumerator DoPlay()
        {
            if(flip == false)
            {
                getSpriteRenderer.flipX = false;
            }
            if (first != null)
            {
                if (flip == true)
                {
                    getSpriteRenderer.flipX = true;
                }
                animator.Play(first.name, 0, 0f);
                yield return null;
                Func<bool> func = () =>
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    return stateInfo.normalizedTime < 1.0f && stateInfo.IsName(first.name) == true;
                };
                yield return new WaitWhile(func);
            }
            if (second != null)
            {
                if (flip == true)
                {
                    getSpriteRenderer.flipX = false;
                }
                animator.Play(second.name, 0, 0f);
                yield return null;
                Func<bool> func = () =>
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    return stateInfo.normalizedTime < 1.0f && stateInfo.IsName(second.name) == true;
                };
                yield return new WaitWhile(func);
            }
            _coroutine = null;
        }
    }

    /// <summary>
    /// Ư�� �ִϸ��̼��� ���� ������ Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="text">Ư�� �ִϸ��̼� �̸�</param>
    /// <returns>�ִϸ��̼� �̸��� ��ġ�ϸ� ���� ��ȯ</returns>
    public bool IsPlaying(string text)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(text);
    }

    /// <summary>
    /// Ư�� �ִϸ��̼��� ���� ������ Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="animationClip">Ư�� �ִϸ��̼� Ŭ��</param>
    /// <returns>�ִϸ��̼� Ŭ���� ��ġ�ϸ� ���� ��ȯ</returns>
    public bool IsPlaying(AnimationClip animationClip)
    {
        return GetCurrentClips() == animationClip;
    }

    /// <summary>
    /// ���� �ִϸ��̼� Ŭ���� �������� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <returns>���� ��� ���� �ִϸ��̼� Ŭ���� ��ȯ��</returns>
    public AnimationClip GetCurrentClips()
    {
        AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        int length = clipInfos != null ? clipInfos.Length : 0;
        if (length > 0)
        {
            return clipInfos[0].clip;
        }
        return null;
    }
}