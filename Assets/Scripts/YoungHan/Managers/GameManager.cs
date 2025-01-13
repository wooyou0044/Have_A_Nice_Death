using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ �Ѱ��ϴ� �Ŵ���, �̱����� ��� ���� Ŭ����
/// </summary>
[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(ObjectPooler))]

public sealed class GameManager : Manager<GameManager>
{
    private bool _hasController = false;

    private Controller _controller = null;

    private Controller getController {
        get
        {
            if (_hasController == false)
            {
                _hasController = true;
                _controller = GetComponent<Controller>();
            }
            return _controller;
        }
    }

    private bool _hasObjectPooler = false;

    private ObjectPooler _objectPooler = null;

    private ObjectPooler getObjectPooler {
        get
        {
            if(_hasObjectPooler == false)
            {
                _hasObjectPooler = true;
                _objectPooler = GetComponent<ObjectPooler>();
            }
            return _objectPooler;
        }
    }

    private List<IHittable> _hittableList = new List<IHittable>();

    protected override void Initialize()
    {
        _destroyOnLoad = true;
        getController._player?.Initialize(Report);
        MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour monoBehaviour in monoBehaviours)
        {
            if (monoBehaviour is IHittable hittable)
            {
                _hittableList.Add(hittable);
            }
        }
        //Strike.Area area = new Strike.TargetArea(new IHittable[] { _hittableList[0], null});
    }

    /// <summary>
    ///  Ư�� ��ġ�� �Ͼ�� ȿ�� ������Ʈ�� �����ִ� �Լ�
    /// </summary>
    /// <param name="original"></param>
    /// <param name="position"></param>
    /// <param name="transform"></param>
    public static void ShowEffect(GameObject original, Vector2 position, Transform transform)
    {
        instance.getObjectPooler.ShowEffect(original, position, transform);
    }

    /// <summary>
    /// ��󿡰� ���� Ȥ�� ȸ���� ���� �󸶳� ���Դ��� �˷��ִ� �Լ�
    /// </summary>
    /// <param name="hittable"></param>
    /// <param name="result"></param>
    public static void Report(IHittable hittable, int result)
    {
        //�������� �� ��󿡰� �󸶳� ���Դ��� �����ϰ� ui�� ���� ����

        if (hittable.isAlive == false)
        {
            //����ϸ� �߰� ����
            if (instance._controller._player == (Object)hittable)
            {

            }
        }
    }

    /// <summary>
    /// ����� Ÿ�� �� �� ȣ���ϴ� �Լ�
    /// </summary>
    /// <param name="strike"></param>
    /// <param name="area"></param>
    /// <param name="effect"></param>
    public static void Report(Strike strike, Strike.Area area, GameObject effect)
    {
        if (area == null)
        {
            foreach (IHittable hittable in instance._hittableList)
            {
                if (hittable != null)
                {
                    instance.getObjectPooler.ShowEffect(effect, hittable.GetCollider2D().bounds.center, hittable.transform);
                    hittable.Hit(strike);
                }
            }
        }
        else
        {
            foreach (IHittable hittable in instance._hittableList)
            {
                if (area.CanStrike(hittable) == true)
                {
                    instance.getObjectPooler.ShowEffect(effect, hittable.GetCollider2D().bounds.center, hittable.transform);
                    hittable.Hit(strike);
                }
            }
        }
    }

    /// <summary>
    /// �̻����� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="adhesion"></param>
    /// <returns></returns>
    public static Projectile GetProjectile(Projectile projectile)
    {
        return instance.getObjectPooler.GetProjectile(projectile);
    }
}