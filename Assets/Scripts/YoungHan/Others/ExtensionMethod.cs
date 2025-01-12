using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 팀원들이 원하는 확장 메서드를 추가로 만들고 싶을 수 있기 때문에 partial로 설정해 놓았다.
/// </summary>
public static partial class ExtensionMethod
{
    public static void SetState(this Parameter parameter, Animator animator, string key)
    {
        if (parameter != null)
        {
            parameter.Set(animator, key);
        }
        else
        {
            animator?.SetTrigger(key);
        }
    }

    public static void Play(this AnimatorHandler animatorHandler, Animator animator)
    {
        animatorHandler?.Play(animator);
    }

    /// <summary>
    /// 사용자가 대상에게 스킬을 쓰게 만드는 확장 메서드
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="user"></param>
    /// <param name="target"></param>
    public static void Use(this Skill skill, ISkillable user, IHittable target)
    {
        if(skill != null)
        {

        }
    }

    /// <summary>
    /// 타격 할 수 있는 대상이 타격 받을 때 쓰는 확장 메서드
    /// </summary>
    /// <param name="hittable"></param>
    /// <param name="strike"></param>
    /// <param name="effect"></param>
    /// <param name="transform"></param>
    /// <param name="action"></param>
    public static void Hit(this IHittable hittable, Strike strike, GameObject effect, Transform transform, Action<GameObject, Vector2, Transform> action)
    {
        if (hittable != null)
        {
            action?.Invoke(effect, hittable.GetCollider2D().bounds.center, transform);
            hittable.Hit(strike);
        }
    }

    /// <summary>
    /// 영역 범위 안의 대상을 타격하는 확장 메서드
    /// </summary>
    /// <param name="area"></param>
    /// <param name="strike"></param>
    /// <param name="hittables"></param>
    /// <param name="action"></param>
    /// <param name="effect"></param>
    /// <param name="transform"></param>
    public static void Hit(this Strike.Area area, Strike strike, IEnumerable<IHittable> hittables, Action<IHittable, Strike, GameObject, Transform> action, GameObject effect, Transform transform)
    {
        if (hittables != null && action != null)
        {
            if (area == null)
            {
                foreach(IHittable hittable in hittables)
                {
                    action.Invoke(hittable, strike, effect, transform);
                }
            }
            else
            {
                foreach (IHittable hittable in hittables)
                {
                    if (area.CanStrike(hittable) == true)
                    {
                        action.Invoke(hittable, strike, effect, transform);
                    }
                }
            }
        }
    }
}