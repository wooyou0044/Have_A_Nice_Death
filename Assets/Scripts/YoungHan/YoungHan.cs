using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoungHan : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Color color;

    private void OnValidate()
    {
        if(spriteRenderer != null)
        {
            spriteRenderer.sharedMaterial.color = color;
        }
    }
    [SerializeField]
    private AnimatorPlayer animatorPlayer;

    [SerializeField]
    AnimationClip clip;

    private void Update()
    {
        if(gameObject.activeSelf == true && animatorPlayer != null)
        {
            animatorPlayer.Play(clip, true);
        }
    }

    /// <summary>
    /// Ư�� ���� �־��� �ݶ��̴�2D ���� �ִ��� Ȯ���մϴ�.
    /// </summary>
    /// <param name="point">Ȯ���� �� (Vector2)</param>
    /// <param name="collider">Ȯ���� Collider2D</param>
    /// <returns>���� �ݶ��̴� ���ο� �ִ��� ���� (bool)</returns>
    public static bool IsPointInsideCollider(Vector2 point, Collider2D collider)
    {
        // Collider2D�� Ÿ�Կ� ���� �浹 ���� �˻�
        if (collider is PolygonCollider2D polygonCollider)
        {
            return IsPointInsidePolygon(point, polygonCollider);
        }
        else if (collider is BoxCollider2D boxCollider)
        {
            return IsPointInsideBox(point, boxCollider);
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            return IsPointInsideCircle(point, circleCollider);
        }

        // �ٸ� Collider2D Ÿ���� �⺻������ ó������ ����
        return false;
    }

    /// <summary>
    /// PolygonCollider2D���� ���� ���ο� �ִ��� Ȯ��
    /// </summary>
    private static bool IsPointInsidePolygon(Vector2 point, PolygonCollider2D polygonCollider)
    {
        // PolygonCollider2D���� ���� �˻縦 �� �� �ִ� ������ ��� (Raycast�� �� ������ ����� ����� ���� ����)
        int hitCount = 0;
        Vector2[] vertices = polygonCollider.points;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 v1 = polygonCollider.transform.TransformPoint(vertices[i]);
            Vector2 v2 = polygonCollider.transform.TransformPoint(vertices[(i + 1) % vertices.Length]);

            // Raycast ������� ���� üũ
            if (IsPointOnLeftSideOfEdge(point, v1, v2))
            {
                hitCount++;
            }
        }

        // Ȧ���̸� ����, ¦���̸� �ܺ�
        return (hitCount % 2 == 1);
    }

    /// <summary>
    /// BoxCollider2D���� ���� ���ο� �ִ��� Ȯ��
    /// </summary>
    private static bool IsPointInsideBox(Vector2 point, BoxCollider2D boxCollider)
    {
        // BoxCollider2D�� AABB�� ����Ͽ� �����ϰ� ���� ���θ� üũ
        Vector2 min = (Vector2)boxCollider.transform.TransformPoint(boxCollider.bounds.min);
        Vector2 max = (Vector2)boxCollider.transform.TransformPoint(boxCollider.bounds.max);

        return point.x >= min.x && point.x <= max.x && point.y >= min.y && point.y <= max.y;
    }

    /// <summary>
    /// CircleCollider2D���� ���� ���ο� �ִ��� Ȯ��
    /// </summary>
    private static bool IsPointInsideCircle(Vector2 point, CircleCollider2D circleCollider)
    {
        // �� ���� üũ
        Vector2 center = (Vector2)circleCollider.transform.TransformPoint(circleCollider.offset);
        float radius = circleCollider.radius;

        return Vector2.Distance(center, point) <= radius;
    }

    /// <summary>
    /// �� �� ���̿� ���� ������ �ִ��� Ȯ���ϴ� �Լ� (�̰��� ������ ���� üũ�� ���� �⺻���� ��� �� �ϳ��Դϴ�)
    /// </summary>
    private static bool IsPointOnLeftSideOfEdge(Vector2 point, Vector2 edgeStart, Vector2 edgeEnd)
    {
        // ������ �������� ���ʿ� ���� �ִ��� Ȯ���ϴ� ��� (Cross Product ���)
        return (edgeEnd.x - edgeStart.x) * (point.y - edgeStart.y) - (edgeEnd.y - edgeStart.y) * (point.x - edgeStart.x) > 0;
    }
}
