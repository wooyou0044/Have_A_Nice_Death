using UnityEngine;

[CreateAssetMenu(menuName = nameof(Shape) + "/Circle")]
public class CircleShape : Shape
{
    public override Strike.PolygonArea GetPolygonArea(Transform transform, string[] tags)
    {
        return null;
    }
}