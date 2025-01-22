using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField]
    private StatusPanel _statusPanel;

    private List<IHittable> _hittableList = new List<IHittable>();
    private List<Ladder> _ladderList = new List<Ladder>();
    private List<ThinGround> _thinGroundList = new List<ThinGround>();

    public static byte soulary = 0;
    public static byte prismium = 0;

    protected override void Initialize()
    {
        _destroyOnLoad = true;
        getController._player?.Initialize(Engage, Report, ShowEffect, Use, TryFalling, TryLadder, GetProjectile);
        MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour monoBehaviour in monoBehaviours)
        {
            if (monoBehaviour is IHittable hittable)
            {
                _hittableList.Add(hittable);
            }
            else if (monoBehaviour is Ladder ladder)
            {
                _ladderList.Add(ladder);
            }
            else if(monoBehaviour is ThinGround thinGround)
            {
                _thinGroundList.Add(thinGround);
            }
        }
    }

    private void Engage(bool escape)
    {
        if (escape == true)
        {
            foreach (Ladder ladder in _ladderList)
            {
                if (ladder.MoveStop() == true)
                {
                    break;
                }
            }
        }
        else
        {
            //아이템 줍기 또는 대화
        }
    }

    private bool TryFalling()
    {
        foreach (ThinGround thinGround in _thinGroundList)
        {
            if (thinGround.MoveDownThinGround() == true)
            {
                return true;
            }
        }
        return false;
    }

    private bool TryLadder(bool rising)
    {
        if (rising == true)
        {
            foreach (Ladder ladder in _ladderList)
            {
                if (ladder.MoveUp() == true)
                {
                    return true;
                }
            }
        }
        else
        {
            foreach (Ladder ladder in _ladderList)
            {
                if (ladder.MoveDown() == true)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    ///  특정 위치에 일어나는 효과 오브젝트를 보여주는 함수
    /// </summary>
    /// <param name="original"></param>
    /// <param name="position"></param>
    /// <param name="transform"></param>
    public static void ShowEffect(GameObject original, Vector2 position, Transform transform)
    {
        instance.getObjectPooler.ShowEffect(original, position, transform);
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
                //게임 오버
            }
            else if (hittable is ILootable lootable)
            {
                MonoBehaviour monoBehaviour = lootable.GetLootObject();
                if (monoBehaviour != null)
                {
                    instance.getObjectPooler.ShowEffect(monoBehaviour.gameObject, hittable.GetCollider2D().bounds.center, null);
                }
            }
        }
        else if (instance._controller._player == (Object)hittable)
        {
            instance._statusPanel?.Set(instance._controller._player);
        }
    }

    /// <summary>
    /// 대상을 타격 할 때 호출하는 함수
    /// </summary>
    /// <param name="strike"></param>
    /// <param name="area"></param>
    /// <param name="effect"></param>
    public static void Use(Strike strike, Strike.Area area, GameObject effect)
    {
        if (area != null)
        {
#if UNITY_EDITOR
            if(area is Strike.PolygonArea polygonArea)
            {
                polygonArea.Show(Color.red, 1);
            }
#endif
            foreach (IHittable hittable in instance._hittableList)
            {
                if (area.CanStrike(hittable) == true && hittable.transform.gameObject.activeInHierarchy == true)
                {
                    instance.getObjectPooler.ShowEffect(effect, hittable.GetCollider2D().bounds.center, hittable.transform);
                    hittable.Hit(strike);
                }
            }
        }
    }

    /// <summary>
    /// 미사일을 반환하는 함수
    /// </summary>
    /// <param name="projectile"></param>
    /// <param name="adhesion"></param>
    /// <returns></returns>
    public static Projectile GetProjectile(Projectile projectile)
    {
        return instance.getObjectPooler.GetProjectile(projectile);
    }
}