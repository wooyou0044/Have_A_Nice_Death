using UnityEngine;

[CreateAssetMenu(menuName = nameof(Shape) + "/Box")]
public class BoxShape : Shape
{
    public override Strike.PolygonArea GetPolygonArea(Transform transform, string[] tags)
    {
        return null;
    }
}
