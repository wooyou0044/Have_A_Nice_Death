using UnityEngine;

/// <summary>
/// Ư�� ��� ���� �浹ü ������� ��ȯ���ִ� Ŭ����
/// </summary>
public abstract class Shape : ScriptableObject
{
    //Collider2D collider = Physics2D.OverlapCircle(getTransform.position, _strikeDistance * 0.5f);
    public abstract void Draw(Transform transform);
}

/// <summary>
/// ��� ���� Ư�� ������ ��ȯ���ִ� Ŭ����
/// </summary>
[CreateAssetMenu(menuName = nameof(CircleShape), order = 0)]
public class CircleShape: Shape
{
    //Collider2D collider = Physics2D.OverlapCircle(getTransform.position, _strikeDistance * 0.5f);
    public override void Draw(Transform transform)
    {

    }
}

/// <summary>
/// ��� ���� Ư�� ������ ��ȯ���ִ� Ŭ����
/// </summary>
[CreateAssetMenu(menuName = nameof(BoxShape), order = 1)]
public class BoxShape : Shape
{
    public override void Draw(Transform transform)
    {

    }
}

/// <summary>
/// ��� ���� Ư�� ������ ��ȯ���ִ� Ŭ����
/// </summary>
[CreateAssetMenu(menuName = nameof(BoxShape), order = 1)]
public class CapsuleShape : Shape
{
    public override void Draw(Transform transform)
    {

    }
}