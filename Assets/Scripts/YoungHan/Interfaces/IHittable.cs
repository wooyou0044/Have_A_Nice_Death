using UnityEngine;
/// <summary>
/// 피격 당할 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface IHittable
{
    public bool isAlive
    {
        get;
    }

    public string tag
    {
        set;
        get;
    }

    public Transform transform
    {
        get;
    }

    public void Hit(Strike strike);

    public Collider2D GetCollider2D();
}