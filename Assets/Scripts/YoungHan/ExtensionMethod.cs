using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ���ϴ� Ȯ�� �޼��带 �߰��� ����� ���� �� �ֱ� ������ partial�� ������ ���Ҵ�.
/// </summary>
public static partial class ExtensionMethod
{
    /// <summary>
    /// Ÿ���� �� �ִ� ����� Ÿ�� ���� �� ���� Ȯ�� �޼���
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
    /// Ÿ�� Ŭ������ �����ϴ� ����� Ÿ���ϴ� Ȯ�� �޼���
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