using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(Skill), order = 0)]
public class Skill : ScriptableObject
{
    [SerializeField, Header("Ÿ�� ��")]
    private Strike strike;
    [SerializeField, Header("���� ���")]
    private Shape shape;

    [Space(10)]
    [SerializeField, Header("����ڰ� �ֵθ��� ȿ��")]
    private GameObject shotObject;
    [SerializeField, Header("��� ������� ���� �ֺ� ȿ��")]
    private GameObject splashObject;
    [SerializeField, Header("������ ����� Ÿ���ϴ� ������Ʈ")]
    private GameObject hitObject;

    [Space(10), SerializeField, Header("�߻�ü")]
    private Projectile projectile;

    /// <summary>
    /// ��ų�� ����� �� ���� �Լ�
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
            //������� Transform ���� �ָ� �ٰŸ����� �ֵθ��� ����
            if (user != null)
            {
                Vector2 position = user.position;
                effectAction.Invoke(shotObject, position, user);
                effectAction.Invoke(splashObject, position, null);
            }
            //�׷��� �ʴٸ� ���Ÿ� ����� ���� ���۽��� ���� ����(Ȥ�� Ÿ���� ���� ��� ����Ʈ�� �� �� �ִ�)
            else if(target != null)
            {
                effectAction.Invoke(splashObject, target.GetCollider2D().bounds.center, null);
            }
        }
        //���� ����� ������
        if (shape != null)
        {
            //������� Transform ���� �ָ� ������� ���� �ȿ��� �������� �ٰŸ� ���÷��� ��
            if (user != null)
            {
                strikeAction?.Invoke(strike, shape.GetPolygonArea(user, new string[] { target.tag }), hitObject);
            }
            //�׷��� ������ Ư�� ��� ������ ���÷��� ��
            else if(target != null)
            {
                strikeAction?.Invoke(strike, shape.GetPolygonArea(target.transform, new string[] { target.tag }), hitObject);
            }
        }
        //���� ����� ������ �� ��� Ÿ��
        else
        {
            strikeAction?.Invoke(strike, new Strike.TargetArea(new IHittable[] { target }), hitObject);
        }
        //�߻�ü�� ��ȯ�ϴ� �Լ��� �ִٸ�
        if (function != null)
        {
            Projectile projectile = function.Invoke(this.projectile);
            projectile?.Shot(user, target, strikeAction, effectAction);
        }
    }


    [Serializable]
    public struct Action : ISerializationCallbackReceiver
    {
        [SerializeField, Header("����� ��ų")]
        private Skill skill;

        [SerializeField, Header("��ų ���� ���ÿ� �����ϴ� �ִϸ�����")]
        private AnimatorHandler animatorHandler;

        [SerializeField, Header("�� ������ �־�� �����ϴ� �ʼ� �ִϸ��̼� Ŭ��")]
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