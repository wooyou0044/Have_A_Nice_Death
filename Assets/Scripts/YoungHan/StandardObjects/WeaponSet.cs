using System;
using UnityEngine;

public class WeaponSet : MonoBehaviour
{


    [SerializeField]
    private Scythe _scytheInfo = null;
    [SerializeField]
    private float _scytheSpeed = 0.6f;

    private float _attackSpeed = 0;

    public bool isPlaying
    {
        get
        {
            return _attackSpeed > 0;
        }
    }

    public void Release()
    {
        if(_attackSpeed > 0)
        {
            _attackSpeed = 0;
        }
    }

    public bool TryScythe(bool pressed, Player.Direction direction, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func, Animator animator)
    {
        if (_scytheInfo != null)
        {
            if (pressed == true)
            {
                if (isPlaying == false)
                {
                    switch (direction)
                    {
                        case Player.Direction.Center:
                            if (_scytheInfo.TryUse(transform, Weapon.Attack.Stand, action1, action2, func, animator) == true)
                            {
                                _attackSpeed = _scytheSpeed;
                                return true;
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
                else
                {
                    //기 모으기
                }
            }
            else if(_attackSpeed > 0)
            {
                _attackSpeed = 0;
            }
        }
        return false;
    }
}
