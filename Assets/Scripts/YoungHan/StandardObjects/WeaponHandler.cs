using System;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField]
    private Scythe _scythe = null;

    public bool TryScythe(bool pressed, Player player, Weapon.Attack attack, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func)
    {
        if (pressed == true)
        {
            Debug.Log("1Ÿ");
            if (_scythe != null)
            {
                return _scythe.TryUse(player, attack, action1, action2, func);
            }
        }
        return false;
    }
}
