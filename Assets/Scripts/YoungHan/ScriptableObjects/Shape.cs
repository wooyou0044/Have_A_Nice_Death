using UnityEngine;

/// <summary>
/// 특정 모양 안의 충돌체 내용들을 반환해주는 클래스
/// </summary>
public abstract class Shape : ScriptableObject
{
    //Collider2D collider = Physics2D.OverlapCircle(getTransform.position, _strikeDistance * 0.5f);
    public abstract void Draw(Transform transform);
}

/// <summary>
/// 모양 안의 특정 내용을 반환해주는 클래스
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
/// 모양 안의 특정 내용을 반환해주는 클래스
/// </summary>
[CreateAssetMenu(menuName = nameof(BoxShape), order = 1)]
public class BoxShape : Shape
{
    public override void Draw(Transform transform)
    {

    }
}

/// <summary>
/// 모양 안의 특정 내용을 반환해주는 클래스
/// </summary>
[CreateAssetMenu(menuName = nameof(BoxShape), order = 1)]
public class CapsuleShape : Shape
{
    public override void Draw(Transform transform)
    {

    }
}