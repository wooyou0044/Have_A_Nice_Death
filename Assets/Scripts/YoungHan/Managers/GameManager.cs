using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 진행을 총괄하는 매니저, 싱글턴을 상속 받은 클래스
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
    }

    private void Hit(IHittable hittable, Strike strike, GameObject effect, Transform transform)
    {
        hittable.Hit(strike, effect, transform, ShowEffect);
    }

    /// <summary>
    ///  특정 위치에 일어나는 효과 오브젝트를 보여주는 함수
    /// </summary>
    /// <param name="original"></param>
    /// <param name="position"></param>
    /// <param name="transform"></param>
    public void ShowEffect(GameObject original, Vector2 position, Transform transform)
    {
        getObjectPooler.Set(original, position, transform);
    }

    /// <summary>
    /// 대상에게 피해 혹은 회복의 값이 얼마나 들어왔는지 알려주는 함수
    /// </summary>
    /// <param name="hittable"></param>
    /// <param name="result"></param>
    public static void Report(IHittable hittable, int result)
    {
        //데미지가 이 대상에게 얼마나 들어왔는지 보고하고 ui로 값을 전송
        if (hittable.isAlive == false)
        {
            //사망하면 추가 보고
            if (instance._controller._player == (Object)hittable)
            {

            }
        }
    }

    /// <summary>
    /// 대상을 타격 할 때 호출하는 함수
    /// </summary>
    /// <param name="target"></param>
    /// <param name="strike"></param>
    /// <param name="effect"></param>
    /// <param name="transform"></param>
    public static void Report(Strike.Target target, Strike strike, Transform transform, GameObject effect)
    {
        target.Hit(strike, instance._hittableList, instance.Hit, effect, transform);
    }

    //스킬을 사용할 때 호출하는 함수
    public static void Report(Skill skill)
    {

    }
}