using System;
using System.Collections;
using UnityEngine;

public class WeaponSet : MonoBehaviour
{
    private enum Combo: byte
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
    private Combo _comboState;
    [SerializeField, Range(0, 5)]
    private float _comboDelay = 0.4f;
    [SerializeField]
    private float _recoverDelay = 1.0f;

    private float _concentrationTime = 0;

    private IEnumerator _coroutine = null;



    public bool TryScythe(Player player, bool pressed, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func)
    {
        if (player != null && player.isAlive == true)
        {
            if (_scytheInfo != null)
            {
                if (pressed == true)
                {
                    if (_concentrationTime == 0)
                    {
                        AnimatorPlayer animatorPlayer = player.animatorPlayer;
                        bool hasAnimatorPlayer = animatorPlayer != null;
                        Animator animator = hasAnimatorPlayer == true ? animatorPlayer.animator : null;
                        if (animator != null)
                        {
                            switch (player.direction)
                            {
                                case Player.Direction.Center:
                                    if (_comboState < Combo.Combo4 && _scytheInfo.TryUse(transform, Weapon.Attack.Stand, action1, action2, func, animator) == true)
                                    {
                                        _comboState++;
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
                                            switch (_comboState)
                                            {
                                                case Combo.Combo1:
                                                case Combo.Combo2:
                                                case Combo.Combo3:
                                                    player.Levitate(_comboDelay);
                                                    yield return new WaitForSeconds(_comboDelay);
                                                    break;
                                                case Combo.Combo4:
                                                    player.Levitate(_comboDelay);
                                                    yield return new WaitForSeconds(_comboDelay);
                                                    player.Dash(Vector2.down, _recoverDelay);
                                                    yield return new WaitForSeconds(_recoverDelay);
                                                    break;
                                            }
                                            player?.Recover();
                                            _comboState = Combo.None;
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
                        _concentrationTime += Time.deltaTime;
                    }
                    else if(_coroutine == null && player.isGrounded == true)
                    {
                        _concentrationTime += Time.deltaTime;
                    }
                }
                else
                {
                    _concentrationTime = 0;
                }
            }
        }
        else if(_concentrationTime > 0)
        {
            _concentrationTime = 0;
        }
        return _coroutine != null || _concentrationTime > 0;
    }
}