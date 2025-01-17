using System;
using System.Collections;
using UnityEngine;

public class WeaponSet : MonoBehaviour
{
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
    private float _comboBaseDelay = 0.4f;
    [SerializeField, Range(0, 5)]
    private float _comboLastDelay = 0.6f;
    [SerializeField, Range(0, 5)]
    private float _comboRecoverDelay = 0.8f;
    [SerializeField]
    private float _concentrationTime = 0;
    [SerializeField]
    private AnimationClip _concentrationClip;

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
                        switch (player.direction)
                        {
                            case Player.Direction.Center:
                                if (_state < State.Combo4 && _scytheInfo.TryUse(transform, Weapon.Attack.Stand, action1, action2, func, animator) == true)
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
                                                player.Levitate(_comboBaseDelay);
                                                yield return new WaitForSeconds(_comboBaseDelay);
                                                break;
                                            case State.Combo4:
                                                player.Levitate(_comboLastDelay);
                                                yield return new WaitForSeconds(_comboLastDelay);
                                                player.Dash(Vector2.down, _comboRecoverDelay);
                                                yield return new WaitForSeconds(_comboRecoverDelay);
                                                break;
                                        }
                                        player?.Recover();
                                        _state = State.None;
                                        _coroutine = null;
                                    }
                                }
                                break;
                            case Player.Direction.Up:
                                if (_scytheInfo.TryUse(transform, Weapon.Attack.Stand_Up, action1, action2, func, animator) == true)
                                {
                                }
                                break;
                            case Player.Direction.Down:
                                break;
                        }
                    }
                }
                else
                {
                   
                    _concentrationTime = 0;
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