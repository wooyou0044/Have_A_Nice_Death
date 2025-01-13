using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(Skill), order = 0)]
public class Skill : ScriptableObject
{
    [SerializeField, Header("타격 값")]
    private Strike strike;
    [SerializeField, Header("범위 모양")]
    private Shape shape;

    [Space(10)]
    [SerializeField, Header("사용자가 휘두르는 효과")]
    private GameObject shotObject;
    [SerializeField, Header("기술 사용으로 인한 주변 효과")]
    private GameObject splashObject;
    [SerializeField, Header("각각의 대상을 타격하는 오브젝트")]
    private GameObject hitObject;

    [Space(10), SerializeField, Header("발사체")]
    private Projectile projectile;

    /// <summary>
    /// 스킬을 사용할 때 쓰는 함수
    /// </summary>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <param name="strikeAction"></param>
    /// <param name="effectAction"></param>
    /// <param name="function"></param>
    public void Use(Transform user, IHittable target, Action<Strike, Strike.Area, GameObject> strikeAction, Action<GameObject, Vector2, Transform> effectAction, Func<Projectile, Projectile> function)
    {
        if (effectAction != null)
        {
            //사용자의 Transform 값을 주면 근거리에서 휘두르는 공격
            if (user != null)
            {
                Vector2 position = user.position;
                effectAction.Invoke(shotObject, position, user);
                effectAction.Invoke(splashObject, position, null);
            }
            //그렇지 않다면 원거리 대상을 향한 갑작스런 광역 공격(혹은 타점에 대한 경고 이펙트를 줄 수 있다)
            else if(target != null)
            {
                effectAction.Invoke(splashObject, target.GetCollider2D().bounds.center, null);
            }
        }
        //공격 모양이 있으면
        if (shape != null)
        {
            //사용자의 Transform 값을 주면 사용자의 범위 안에서 행해지는 근거리 스플래쉬 딜
            if (user != null)
            {
                strikeAction?.Invoke(strike, shape.GetPolygonArea(user, new string[] { target.tag }), hitObject);
            }
            //그렇지 않으면 특정 대상에 유착된 스플래쉬 딜
            else if(target != null)
            {
                strikeAction?.Invoke(strike, shape.GetPolygonArea(target.transform, new string[] { target.tag }), hitObject);
            }
        }
        //공격 모양이 없으면 그 대상만 타격
        else
        {
            strikeAction?.Invoke(strike, new Strike.TargetArea(new IHittable[] { target }), hitObject);
        }
        //발사체를 반환하는 함수가 있다면
        if (function != null)
        {
            Projectile projectile = function.Invoke(this.projectile);
            projectile?.Shot(user, target, strikeAction, effectAction);
        }
    }


    [Serializable]
    public struct Action : ISerializationCallbackReceiver
    {
        [SerializeField, Header("사용할 스킬")]
        private Skill skill;

        [SerializeField, Header("스킬 사용과 동시에 동작하는 애니메이터")]
        private AnimatorHandler animatorHandler;

        [SerializeField, Header("이 내용이 있어야 동작하는 필수 애니메이션 클립")]
        private List<AnimationClip> essentialClips;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            int count = essentialClips.Count;
            if (count > 0)
            {
                List<AnimationClip> list = new List<AnimationClip>();
                for (int i = 0; i < count; i++)
                {
                    if (essentialClips[i] != null && list.Contains(essentialClips[i]) == false)
                    {
                        list.Add(essentialClips[i]);
                    }
                    else if (i == count - 1)
                    {
                        if (list.Contains(essentialClips[i]) == true)
                        {
                            list.Add(null);
                        }
                        else
                        {
                            list.Add(essentialClips[i]);
                        }
                    }
                }
                essentialClips = list;
            }
        }

        public bool TryUse(Animator animator, Transform user, IHittable target, Action<Strike, Strike.Area, GameObject> strikeAction, Action<GameObject, Vector2, Transform> effectAction, Func<Projectile, Projectile> function)
        {
            int count = essentialClips.Count;
            if (animator != null)
            {
                if (count > 0)
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    for (int i = 0; i < count; i++)
                    {
                        if (essentialClips[i] != null && stateInfo.IsName(essentialClips[i].name) == true && stateInfo.normalizedTime >= 1.0)
                        {
                            animatorHandler?.Play(animator);
                            skill?.Use(user, target, strikeAction, effectAction, function);
                            return true;
                        }
                    }
                }
                else
                {
                    animatorHandler?.Play(animator);
                    skill?.Use(user, target, strikeAction, effectAction, function);
                    return true;
                }
            }
            else if (count == 0)
            {
                skill?.Use(user, target, strikeAction, effectAction, function);
                return true;
            }
            return false;
        }
    }
}