using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 원하는 애니메이션을 동작시킬 수 있는 클래스
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
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
    /// 애니메이션 진행이 다 끝났는지 확인하는 프로퍼티
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
    /// 강제로 애니메이션을 멈추게 만드는 함수
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
    /// 애니메이터 Trigger를 재생시키는 함수
    /// </summary>
    /// <param name="key"></param>
    public void Play(string key)
    {
        animator.SetTrigger(key);
    }

    /// <summary>
    /// 애니메이터 Bool을 재생시키는 함수
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Play(string key, bool value)
    {
        animator.SetBool(key, value);
    }

    /// <summary>
    /// 애니메이터 Int를 재생시키는 함수
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Play(string key, int value)
    {
        animator.SetInteger(key, value);
    }

    /// <summary>
    /// 애니메이터 Float을 재생시키는 함수
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Play(string key, float value)
    {
        animator.SetFloat(key, value);
    }

    /// <summary>
    /// 특정 애니메이션을 재생 시키는 함수
    /// </summary>
    /// <param name="animationClip">재생할 애니메이션 클립</param>
    /// <param name="force">true일 경우 기존에 진행 중인 애니메이션 재생을 취소하고 새롭게 재생</param>
    public void Play(AnimationClip animationClip, bool force = true)
    {
        Play(animationClip, null, false, force);
    }

    /// <summary>
    /// 특정 애니메이션 두 개를 순차적으로 재생 시키는 함수
    /// </summary>
    /// <param name="first">재생할 첫 번째 애니메이션</param>
    /// <param name="second">재생할 두 번째 애니메이션</param>
    /// <param name="flip">스프라이트 렌더러를 일시적으로 반전 시켰다가 원래대로 되돌림</param>
    /// <param name="force">true일 경우 기존에 진행 중인 애니메이션 재생을 취소하고 새롭게 재생</param>
    public void Play(AnimationClip first, AnimationClip second, bool flip, bool force = true)
    {
        if (enabled == false && Application.isPlaying == true)
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
    /// 특정 애니메이션이 진행 중인지 확인하는 함수
    /// </summary>
    /// <param name="text">특정 애니메이션 이름</param>
    /// <returns>애니메이션 이름이 일치하면 참을 반환</returns>
    public bool IsPlaying(string text)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(text);
    }

    /// <summary>
    /// 특정 애니메이션이 진행 중인지 확인하는 함수
    /// </summary>
    /// <param name="animationClip">특정 애니메이션 클립</param>
    /// <returns>애니메이션 클립이 일치하면 참을 반환</returns>
    public bool IsPlaying(AnimationClip animationClip)
    {
        return GetCurrentClips() == animationClip;
    }

    /// <summary>
    /// 현재 애니메이션 클립이 무엇인지 반환하는 함수
    /// </summary>
    /// <returns>현재 재생 중인 애니메이션 클립을 반환함</returns>
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