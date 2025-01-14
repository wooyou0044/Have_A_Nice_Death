using System;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(Weapon), order = 0)]
public abstract class Weapon : ScriptableObject
{
    [SerializeField]
    private Skill.Action defaultAction;

    public enum Style: byte
    {
        Default,
    }


    public bool TryUse(Player player, Style style, IHittable target, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2, Func<Projectile, Projectile> func)
    {

        return false;
    }
}