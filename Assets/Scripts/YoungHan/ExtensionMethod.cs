using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 팀원들이 원하는 확장 메서드를 추가로 만들고 싶을 수 있기 때문에 partial로 설정해 놓았다.
/// </summary>
public static partial class ExtensionMethod
{
    /// <summary>
    /// 타격할 수 있는 대상이 타격 당할 때 쓰는 확장 메서드
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
    /// 타겟 클래스가 지목하는 대상을 타격하는 확장 메서드
    /// </summary>
    /// <param name="target"></param>
    /// <param name="strike"></param>
    /// <param name="hittables"></param>
    /// <param name="action"></param>
    /// <param name="effect"></param>
    /// <param name="transform"></param>
    public static void Hit(this Strike.Target target, Strike strike, IEnumerable<IHittable> hittables, Action<IHittable, Strike, GameObject, Transform> action, GameObject effect, Transform transform)
    {
        if (hittables != null && action != null)
        {
            if (target == null)
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
                    if (target.CanStrike(hittable) == true)
                    {
                        action.Invoke(hittable, strike, effect, transform);
                    }
                }
            }
        }
    }
}