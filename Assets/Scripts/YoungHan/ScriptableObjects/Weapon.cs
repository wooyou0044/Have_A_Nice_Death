using System;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(Weapon), order = 0)]
public class Weapon : ScriptableObject
{
    [SerializeField]
    private Skill.Action action1;

    //평타1
    //평타2
    //





    public bool TryUse(Transform user, IHittable target, Action<Strike, Strike.Area, GameObject> strike, Action<GameObject, Vector2, Transform> effect, Func<Projectile, Projectile> projectile, AnimatorPlayer animatorPlayer)
    {
        //return action1.TryUse(animator, user, target, strike, effectAction, function);
        return false;
    }
}