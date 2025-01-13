using UnityEngine;
/// <summary>
/// �ǰ� ���� �� �ִ� ��ü���� ��� �޴� �������̽�
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