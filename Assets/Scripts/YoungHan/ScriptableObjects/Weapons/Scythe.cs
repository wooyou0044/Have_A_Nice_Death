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

    public override bool TryUse(Player player, Attack attack, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func)
    {
        switch(attack)
        {
            case Attack.Stand:
                if(standAction1.TryUse(player.transform, null, action1, action2, func, player.animator) == true)
                {
                    return true;
                }
                //if (standAction2.TryUse(player.transform, null, action1, action2, func, player.animator) == true)
                //{
                //    return true;
                //}
                //if (standAction3.TryUse(player.transform, null, action1, action2, func, player.animator) == true)
                //{
                //    return true;
                //}
                //if (standAction4.TryUse(player.transform, null, action1, action2, func, player.animator) == true)
                //{
                //    return true;
                //}
                break;
        }
        return false;
    }
}