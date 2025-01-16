using System;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(Weapon) + "/Scythe")]
public class Scythe : Weapon
{
    [SerializeField]
    private Skill.Action standAction1;
    [SerializeField]
    private Skill.Action standAction2;
    [SerializeField]
    private Skill.Action standAction3;
    [SerializeField]
    private Skill.Action standAction4;

    public override bool TryUse(Transform transform, Attack attack, float duration, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func, Animator animator)
    {
        switch (attack)
        {
            case Attack.Stand:
                if (standAction4.TryUse(transform, null, duration, action1, action2, func, animator) == true)
                {
                    return true;
                }
                if (standAction3.TryUse(transform, null, duration, action1, action2, func, animator) == true)
                {
                    return true;
                }
                if (standAction2.TryUse(transform, null, duration, action1, action2, func, animator) == true)
                {
                    return true;
                }
                if (standAction1.TryUse(transform, null, duration, action1, action2, func, animator) == true)
                {
                    return true;
                }
                break;
        }
        return false;
    }
}