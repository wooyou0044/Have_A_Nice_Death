using System;
using System.Collections;
using UnityEngine;

public class WeaponSet : MonoBehaviour
{

    [SerializeField, Header("낫 정보")]
    private Scythe _scytheInfo = null;

    private float _concentrationTime = 0;

    private enum Combo
    {
        None,
        Combo1,
        Combo2,
        Combo3,
        Combo4
    }

    [SerializeField]
    private Combo _combo;

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
                    Animator animator = animatorPlayer != null ? animatorPlayer.animator : null;
                    if (animator != null)
                    {
                        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                        switch (player.direction)
                        {
                            case Player.Direction.Center:
                                if (_combo < Combo.Combo4 && _scytheInfo.TryUse(transform, Weapon.Attack.Stand, stateInfo.length * stateInfo.normalizedTime, action1, action2, func, animator) == true)
                                {
                                    _combo++;
                                    Debug.Log(_combo);
                                    if(_coroutine != null)
                                    {
                                        StopCoroutine(_coroutine);
                                    }
                                    _coroutine = DoPlay();
                                    StartCoroutine(_coroutine);
                                    IEnumerator DoPlay()
                                    {
                                        switch(_combo)
                                        {
                                            case Combo.Combo1:
                                                yield return new WaitForSeconds(0.4f);
                                                break;
                                            case Combo.Combo2:
                                                yield return new WaitForSeconds(0.4f);
                                                break;
                                            case Combo.Combo3:
                                                yield return new WaitForSeconds(0.4f);
                                                break;
                                            case Combo.Combo4:
                                                yield return new WaitForSeconds(2f);
                                                break;
                                        }
                                        _combo = Combo.None;
                                        _coroutine = null;
                                    }
                                }
                                break;
                            case Player.Direction.Up:
                                if (_scytheInfo.TryUse(transform, Weapon.Attack.Stand_Up, 0, action1, action2, func, animator) == true)
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

                }
            }
        }
        else if(_concentrationTime > 0)
        {
            _concentrationTime = 0;
        }
        //Animator animator = _animatorPlayer != null ? _animatorPlayer.animator : null;
        //if (getWeaponSet.TryScythe(pressed, _direction, _effectAction, _strikeAction, _projectileFunction, animator) == true)
        //{
        //    if (_coroutine != null)
        //    {
        //        StopCoroutine(_coroutine);
        //    }
        //    _coroutine = DoPlay();
        //    StartCoroutine(_coroutine);
        //    IEnumerator DoPlay()
        //    {
        //        if (pressed == true)
        //        {
        //            if (_animatorPlayer != null)
        //            {
        //                _animatorPlayer.Flip(false);
        //                _animatorPlayer.Stop();
        //            }
        //            Levitate(true, _levitateTime);
        //            yield return new WaitForSeconds(_levitateTime);
        //            if (isGrounded == true)
        //            {
        //                _animatorPlayer?.Play(_idleClip);
        //            }
        //            else
        //            {
        //                _animatorPlayer?.Play(_jumpFallingClip);
        //            }
        //        }
        //        else
        //        {
        //            //하강 공격
        //        }
        //        _coroutine = null;
        //    }
        //}
        return _combo > Combo.None;
    }
}
