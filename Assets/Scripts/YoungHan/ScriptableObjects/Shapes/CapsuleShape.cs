using UnityEngine;

[CreateAssetMenu(menuName = nameof(Shape) + "/Capsule")]
public class CapsuleShape : Shape
{
    public override Strike.PolygonArea GetPolygonArea(Transform transform, string[] tags)
    {
        return null;
    }
}
