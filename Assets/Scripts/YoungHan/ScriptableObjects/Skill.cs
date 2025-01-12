using System;
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
    /// <param name="hitAction"></param>
    /// <param name="effectAction"></param>
    /// <param name="function"></param>
    public void Use(Transform user, IHittable target, Action<Strike, Strike.Area, GameObject> hitAction, Action<GameObject, Vector2, Transform> effectAction, Func<Projectile, Projectile> function)
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
        //���� ����� �ְ�
        if(target != null)
        {
            //���� ����� ������
            if (shape != null)
            {
                //������� Transform ���� �ָ� ������� ���� �ȿ��� �������� �ٰŸ� ���÷��� ��
                if (user != null)
                {
                    hitAction?.Invoke(strike, shape.GetPolygonArea(user, new string[] {target.tag}), hitObject);
                }
                //�׷��� ������ Ư�� ��� ������ ���÷��� ��
                else
                {
                    hitAction?.Invoke(strike, shape.GetPolygonArea(target.GetCollider2D().transform, new string[] { target.tag }), hitObject);
                }
            }
            //���� ����� ������ �� ��� Ÿ��
            else
            {
                hitAction?.Invoke(strike, new Strike.TargetArea(new IHittable[] { target }), hitObject);
            }
        }
        //�װ͵� �ƴ϶�� ��� ��� Ÿ��
        else
        {
            hitAction?.Invoke(strike, null, hitObject);
        }
        //�߻�ü�� �ִٸ�
        if(function != null)
        {
            Projectile projectile = function.Invoke(this.projectile);
            if(projectile != null)
            {

            }
        }
    }
}