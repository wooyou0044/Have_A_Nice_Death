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
    /// 특정 점이 주어진 콜라이더2D 내에 있는지 확인합니다.
    /// </summary>
    /// <param name="point">확인할 점 (Vector2)</param>
    /// <param name="collider">확인할 Collider2D</param>
    /// <returns>점이 콜라이더 내부에 있는지 여부 (bool)</returns>
    public static bool IsPointInsideCollider(Vector2 point, Collider2D collider)
    {
        // Collider2D의 타입에 따라 충돌 여부 검사
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

        // 다른 Collider2D 타입은 기본적으로 처리하지 않음
        return false;
    }

    /// <summary>
    /// PolygonCollider2D에서 점이 내부에 있는지 확인
    /// </summary>
    private static bool IsPointInsidePolygon(Vector2 point, PolygonCollider2D polygonCollider)
    {
        // PolygonCollider2D에서 내부 검사를 할 수 있는 간단한 방법 (Raycast나 더 복잡한 방법을 사용할 수도 있음)
        int hitCount = 0;
        Vector2[] vertices = polygonCollider.points;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 v1 = polygonCollider.transform.TransformPoint(vertices[i]);
            Vector2 v2 = polygonCollider.transform.TransformPoint(vertices[(i + 1) % vertices.Length]);

            // Raycast 방식으로 내부 체크
            if (IsPointOnLeftSideOfEdge(point, v1, v2))
            {
                hitCount++;
            }
        }

        // 홀수이면 내부, 짝수이면 외부
        return (hitCount % 2 == 1);
    }

    /// <summary>
    /// BoxCollider2D에서 점이 내부에 있는지 확인
    /// </summary>
    private static bool IsPointInsideBox(Vector2 point, BoxCollider2D boxCollider)
    {
        // BoxCollider2D의 AABB를 사용하여 간단하게 내부 여부를 체크
        Vector2 min = (Vector2)boxCollider.transform.TransformPoint(boxCollider.bounds.min);
        Vector2 max = (Vector2)boxCollider.transform.TransformPoint(boxCollider.bounds.max);

        return point.x >= min.x && point.x <= max.x && point.y >= min.y && point.y <= max.y;
    }

    /// <summary>
    /// CircleCollider2D에서 점이 내부에 있는지 확인
    /// </summary>
    private static bool IsPointInsideCircle(Vector2 point, CircleCollider2D circleCollider)
    {
        // 원 내부 체크
        Vector2 center = (Vector2)circleCollider.transform.TransformPoint(circleCollider.offset);
        float radius = circleCollider.radius;

        return Vector2.Distance(center, point) <= radius;
    }

    /// <summary>
    /// 두 점 사이에 점이 좌측에 있는지 확인하는 함수 (이것은 폴리곤 내부 체크를 위한 기본적인 방법 중 하나입니다)
    /// </summary>
    private static bool IsPointOnLeftSideOfEdge(Vector2 point, Vector2 edgeStart, Vector2 edgeEnd)
    {
        // 선분을 기준으로 왼쪽에 점이 있는지 확인하는 방법 (Cross Product 사용)
        return (edgeEnd.x - edgeStart.x) * (point.y - edgeStart.y) - (edgeEnd.y - edgeStart.y) * (point.x - edgeStart.x) > 0;
    }
}
