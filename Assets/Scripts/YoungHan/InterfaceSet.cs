using UnityEngine;



/// <summary>
/// Ư�� ��ġ���� ��ȯ�ؾ� �Ǵ� ��Ȳ�� ���� �������̽�
/// </summary>
public interface IPositionable
{
    //��ġ ������ ������ ������Ƽ
    Vector2 position {
        get;
        set;
    }
}

/// <summary>
/// ������ �Ǵ� �������� �̵��ϰų� �ٶ� �� �ִ� ��ü���� ��� �޴� �������̽�
/// </summary>
public interface IMovable
{
    //�ٸ� ��ü�� �浹 �� ��ȿ �Ÿ�
    protected static readonly float OverlappingDistance = 0.02f;
    //���� ���� �� ���� ��Ż�ߴٰ� �����ϴ� �ּ� �ӵ�
    protected static readonly float MinimumDropVelocity = -0.49f;
    //���������� �̵�
    public void MoveRight();
    //�������� �̵�
    public void MoveLeft();
    //�̵� ����
    public void MoveStop();
}

/// <summary>
/// �پ� ���� �� �ִ� ��ü���� ��� �޴� �������̽�
/// </summary>
public interface IJumpable
{
    //�پ� ������
    public void Jump();
    //�� �� �ִ��� ��ȯ
    public bool CanJump();
}

/// <summary>
/// �ǰ� ���� �� �ִ� ��ü���� ��� �޴� �������̽�
/// </summary>
public interface IHittable
{
    public bool isAlive {
        get;
    }

    public string tag
    {
        set;
        get;
    }

    public void Hit(Strike strike);

    //public void Hit(Spell[] spells);

    public Collider2D GetCollider2D();
}

/// <summary>
/// ��ȣ �ۿ� �� �� �ִ� ��ü���� ��� �޴� �������̽�
/// </summary>
public interface Iinteractable
{
    public bool CanInteraction(Vector2 position);
}