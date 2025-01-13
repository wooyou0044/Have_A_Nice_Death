using System;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(Weapon), order = 0)]
public class Weapon : ScriptableObject
{
    [SerializeField]
    private Skill.Action action1;




    public bool TryUse(Animator animator,Transform user, IHittable target, Action<Strike, Strike.Area, GameObject> strikeAction, Action<GameObject, Vector2, Transform> effectAction, Func<Projectile, Projectile> function)
    {
        return action1.TryUse(animator, user, target, strikeAction, effectAction, function);
    }
}