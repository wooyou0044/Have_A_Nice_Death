using System;
using System.Collections;
using UnityEngine;

public class WeaponSet : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    protected Transform getTransform
    {
        get
        {
            if (_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    private enum State: byte
    {
        None,
        Combo1,
        Combo2,
        Combo3,
        Combo4,
    }

    [SerializeField, Header("³´ Á¤º¸")]
    private Scythe _scytheInfo = null;
    [SerializeField, Header("ÄÞº¸ È½¼ö")]
    private State _state;
    [SerializeField, Range(0, 5)]
    private float _comboBaseDelay = 0.5f;
    [SerializeField, Range(0, 5)]
    private float _comboDashDelay = 0.05f;
    [SerializeField, Range(0, 5)]
    private float _comboLastDelay = 0.7f;
    [SerializeField, Range(0, 1)]
    private float _jumpStartDelay = 0.1f;
    [SerializeField, Range(0, 1)]
    private float _jumpFallingDelay = 0.3f;
    [SerializeField, Range(0, 1)]
    private float _jumpLandingDelay = 0.4f;

    [SerializeField]
    private float _concentrationTime = 0;
    [SerializeField]
    private AnimationClip _concenteStartClip;
    [SerializeField]
    private AnimationClip _concenteLoopClip;
    [SerializeField]
    private AnimationClip _restAttackClip;
    [SerializeField]
    private Skill _restAttackSkill;
    [SerializeField]
    private string[] _restAttackTags;

    private IEnumerator _coroutine = null;

    public bool TryScythe(Player player, bool pressed, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func)
    {
        if (player != null && player.isAlive == true)
        {
            if (_scytheInfo != null)
            {
                if (pressed == true)
                {
                    AnimatorPlayer animatorPlayer = player.animatorPlayer;
                    bool hasAnimatorPlayer = animatorPlayer != null;
                    Animator animator = hasAnimatorPlayer == true ? animatorPlayer.animator : null;
                    if (animator != null)
                    {
                        if (_concentrationTime == 0)
                        {
                            switch (player.direction)
                            {
                                case Player.Direction.Center:
                                    if (_state < State.Combo4 && player.CompareConstraints(RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation) == false &&
                                        _scytheInfo.TryUse(transform, player.isGrounded == true ? Weapon.Attack.Move : Weapon.Attack.Stand, action1, action2, func, animator) == true)
                                    {
                                        if (hasAnimatorPlayer == true)
                                        {
                                            animatorPlayer.Flip(false);
                                            animatorPlayer.Stop();
                                        }
                                        if (_coroutine != null)
                                        {
                                            StopCoroutine(_coroutine);
                                        }
                                        _coroutine = DoPlay();
                                        StartCoroutine(_coroutine);
                                        IEnumerator DoPlay()
                                        {
                                            _state++;
                                            switch (_state)
                                            {
                                                case State.Combo1:
                                                case State.Combo2:
                                                case State.Combo3:
                                                    if (player.isGrounded == false)
                                                    {
                                                        player.Dash(Vector2.zero, _comboBaseDelay);
                                                        yield return new WaitUntil(() => player!= null && player.isGrounded == true);
                                                    }
                                                    else
                                                    {
                                                        player.Dash(_comboDashDelay);
                                                        yield return new WaitForSeconds(_comboBaseDelay);
                                                    }
                                                    break;
                                                case State.Combo4:
                                                    yield return new WaitForSeconds(_comboLastDelay);
                                                    break;
                                            }
                                            player?.Recover();
                                            _state = State.None;
                                            _coroutine = null;
                                        }
                                    }
                                    break;
                                case Player.Direction.Forward:
                                    if (_scytheInfo.TryUse(transform, Weapon.Attack.Move_Up, action1, action2, func, animator) == true)
                                    {
                                    }
                                    break;
                                case Player.Direction.Backward:
                                    break;
                            }
                        }
                        else if(_concentrationTime > _comboBaseDelay && player.isGrounded == true && _coroutine == null && _state == State.None)
                        {
                            _coroutine = DoPlay();
                            StartCoroutine(_coroutine);
                            IEnumerator DoPlay()
                            {
                                animatorPlayer.Play(_concenteStartClip, _concenteLoopClip);
                                while(player != null && player.isGrounded == true)
                                {
                                    yield return null;
                                }
                                _coroutine = null;
                            }
                        }
                    }
                    _concentrationTime += Time.deltaTime;
                }
                else
                {
                    if (_coroutine != null && _state == State.None)
                    {
                        AnimatorPlayer animatorPlayer = player.animatorPlayer;
                        if (animatorPlayer != null)
                        {
                            AnimationClip animationClip = animatorPlayer.GetCurrentClips();
                            if(animationClip == _concenteStartClip || animationClip == _concenteLoopClip)
                            {
                                StopCoroutine(_coroutine);
                                _coroutine = DoPlay();
                                StartCoroutine(_coroutine);
                                IEnumerator DoPlay()
                                {
                                    animatorPlayer.Play(_restAttackClip);
                                    player.Dash(Vector2.up, _jumpStartDelay);
                                    yield return new WaitForSeconds(_jumpStartDelay);
                                    player?.Dash(Vector2.zero, _jumpFallingDelay);
                                    yield return new WaitForSeconds(_jumpFallingDelay);
                                    player?.Dash(Vector2.down);
                                    yield return new WaitUntil(() => (player != null && player.isGrounded == true));
                                    _restAttackSkill?.Use(getTransform, null, _restAttackTags, action1, action2, func);
                                    yield return new WaitForSeconds(_jumpLandingDelay);
                                    player?.Recover();
                                    _coroutine = null;
                                    _concentrationTime = 0;
                                }
                            }
                        }
                        else
                        {
                            player.Recover();
                            StopCoroutine(_coroutine);
                            _coroutine = null;
                            _concentrationTime = 0;
                        }
                    }
                    else if(_concentrationTime > 0)
                    {
                        _concentrationTime = 0;
                    }
                }
            }
        }
        else
        {
            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            if (_concentrationTime > 0)
            {
                _concentrationTime = 0;
            }
        }
        return _coroutine != null;
    }
}