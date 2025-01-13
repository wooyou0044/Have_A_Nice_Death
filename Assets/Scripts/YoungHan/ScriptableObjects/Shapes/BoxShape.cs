using UnityEngine;

[CreateAssetMenu(menuName = nameof(Shape) + "/Box")]
public sealed class BoxShape : Shape
{
    [SerializeField, Header("ũ��")]
    private Vector2 size;

    [SerializeField, Header("������")]
    private Vector2 offset;

    /// <summary>
    /// �簢���� ������ ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public override Strike.PolygonArea GetPolygonArea(Transform transform, string[] tags)
    {
        if (transform != null)
        {
            int dotCount = 4;
            float half = 0.5f;
            Vector2[] localEdges = new Vector2[]
            {
                offset + new Vector2(-size.x * half, -size.y * half),
                offset + new Vector2(size.x * half, -size.y * half),
                offset + new Vector2(size.x * half, size.y * half),
                offset + new Vector2(-size.x * half, size.y * half),
            };
            Vector2[] vertices = new Vector2[dotCount * 2];
            for (int i = 0; i < dotCount; i++)
            {
                vertices[i * 2] = transform.TransformPoint(localEdges[i]);
                vertices[i * 2 + 1] = transform.TransformPoint(localEdges[(i + 1) % dotCount]);
            }
            return new Strike.PolygonArea(vertices, tags);
        }
        return null;
    }
}
