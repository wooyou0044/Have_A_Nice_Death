using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ���ϴ� Ȯ�� �޼��带 �߰��� ����� ���� �� �ֱ� ������ partial�� ������ ���Ҵ�.
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
    /// ����ڰ� ��󿡰� ��ų�� ���� ����� Ȯ�� �޼���
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
    /// Ÿ�� �� �� �ִ� ����� Ÿ�� ���� �� ���� Ȯ�� �޼���
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
    /// ���� ���� ���� ����� Ÿ���ϴ� Ȯ�� �޼���
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