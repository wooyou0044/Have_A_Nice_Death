using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ �Ѱ��ϴ� �Ŵ���, �̱����� ��� ���� Ŭ����
/// </summary>
public sealed class GameManager : Manager<GameManager>
{
    [SerializeField]
    private Controller _controller1;

    private List<IHittable> _hittableList = new List<IHittable>();

    protected override void Initialize()
    {
        _destroyOnLoad = true;
        _controller1.player.Initialize(Report, CanInteraction);
    }

    private void ShowEffect(GameObject effectObject, Vector2 position)
    {

    }

    private void Hit(Strike strike, IHittable hittable, GameObject effectObject)
    {
        ShowEffect(effectObject, hittable.GetCollider2D().bounds.center);
        hittable.Hit(strike);
    }

    public static void Report(IHittable hittable, int strikeResult)
    {
        //�������� �� ��󿡰� �󸶳� ���Դ��� �����ϰ� ui�� ���� ����
        //����ϸ� ���� ����
    }

    public static void Report(Strike strike, Strike.Area area, GameObject effectObject)
    {
        if(area == null)
        {
            instance.Hit(strike, instance._controller1.player, effectObject);
            int count = instance._hittableList.Count;
            for (int i = 0; i < count; i++)
            {
                instance.Hit(strike, instance._hittableList[i], effectObject);
            }
        }
        else
        {
            if (area.CanStrike(instance._controller1.player) == true)
            {
                instance.Hit(strike, instance._controller1.player, effectObject);
            }
            int count = instance._hittableList.Count;
            for (int i = 0; i < count; i++)
            {
                if (area.CanStrike(instance._hittableList[i]) == true)
                {
                    instance.Hit(strike, instance._hittableList[i], effectObject);
                }
            }
        }
    }

    public static bool CanInteraction(Player.Interaction interaction)
    {

        return false;
    }
}