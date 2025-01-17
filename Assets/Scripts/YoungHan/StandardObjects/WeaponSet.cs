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
    private float _comboBaseDelay = 0.5f;
    [SerializeField, Range(0, 5)]
    private float _comboLastDelay = 0.7f;
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
                    if (_concentrationTime <= 0)
                    {
                        AnimatorPlayer animatorPlayer = player.animatorPlayer;
                        bool hasAnimatorPlayer = animatorPlayer != null;
                        Animator animator = hasAnimatorPlayer == true ? animatorPlayer.animator : null;
                        if (animator != null)
                        {
                            switch (player.direction)
                            {
                                case Player.Direction.Center:
                                    if (_state < State.Combo4 && _scytheInfo.TryUse(transform, player.isGrounded == true ? Weapon.Attack.Move : Weapon.Attack.Stand, action1, action2, func, animator) == true)
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
                                        _state++;
                                        if (player.isGrounded == true)
                                        {
                                            player.Dash(new Vector2(player.transform.forward.normalized.z, 0), _comboBaseDelay);
                                        }

                                        _coroutine = DoPlay();
                                        StartCoroutine(_coroutine);
                                        IEnumerator DoPlay()
                                        {
                                            switch (_state)
                                            {
                                                case State.Combo1:
                                                case State.Combo2:
                                                case State.Combo3:
                                                
                                                    yield return new WaitForSeconds(_comboBaseDelay);
                                                    break;
                                                case State.Combo4:
                                                    //player.Levitate(_comboLastDelay);
                                                    yield return new WaitForSeconds(_comboLastDelay);
                                                    //player.Dash(Vector2.down, _comboRecoverDelay);
                                                    break;
                                            }
                                            player?.Recover();
                                            _state = State.None;
                                            _coroutine = null;
                                        }
                                    }
                                    break;
                                case Player.Direction.Up:
                                    if (_scytheInfo.TryUse(transform, Weapon.Attack.Move_Up, action1, action2, func, animator) == true)
                                    {
                                    }
                                    break;
                                case Player.Direction.Down:
                                    break;
                            }
                        }
                    }
                    else if(player.isGrounded == true)
                    {

                    }
                    _concentrationTime += Time.deltaTime;
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