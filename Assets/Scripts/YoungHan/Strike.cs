using UnityEngine;

/// <summary>
/// 대상을 타격하는 구조체
/// </summary>
public struct Strike
{
    //타격하는 강도
    private int power;

    //만약 타격 용도가 공격이라면 공격량을 확장하는 용도
    private byte extension;

    //최종적으로 대상에게 가해질 결과값을 반환한다.
    public int result
    {
        get
        {
            //power가 음수이면 공격을 위한 용도
            if (power < 0)
            {
                return power - Random.Range(0, extension);
            }
            //power가 양수이면 회복을 시키기 위한 용도
            else
            {
                return power;
            }
        }
    }

    public Strike(int power, byte extension)
    {
        this.power = power;
        this.extension = extension;
    }

    /// <summary>
    /// 해당 대상이 타격 대상인지 구별하는 클래스(단 이 값이 null이라면 대상을 구별하지 않고 광역 타격)
    /// </summary>
    public abstract class Target
    {
#if UNITY_EDITOR
        private static readonly float DotSize = 0.01f;
        protected static readonly float DrawDuration = 3;

        protected static void DrawDot(Vector2 point, Color color, float duration)
        {
            float half = DotSize * 0.5f;
            Vector2 dot1 = new Vector2(point.x + half, point.y + half);
            Vector2 dot2 = new Vector2(point.x - half, point.y + half);
            Vector2 dot3 = new Vector2(point.x - half, point.y - half);
            Vector2 dot4 = new Vector2(point.x + half, point.y - half);
            Debug.DrawLine(dot1, dot2, color, duration);
            Debug.DrawLine(dot2, dot3, color, duration);
            Debug.DrawLine(dot3, dot4, color, duration);
            Debug.DrawLine(dot4, dot1, color, duration);
        }
#endif
        public abstract void Show();

        public abstract bool CanStrike(IHittable hittable);
    }

    /// <summary>
    /// 특정 태그만 타격 대상으로 간주하는 클래스
    /// </summary>
    public class TagTarget : Target
    {
        private string[] tags;

        public TagTarget(string[] tags)
        {
            this.tags = tags;
        }


        public override void Show()
        {
#if UNITY_EDITOR
            int length = tags != null ? tags.Length : 0;
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    Debug.Log($"<color=red>타격 대상:{tags[i]}</color>");
                }
            }
            else
            {
                Debug.Log($"<color=black>타격 대상 없음</color>");
            }
#endif
        }

        public override bool CanStrike(IHittable hittable)
        {
            if (hittable != null)
            {
                int length = tags != null ? tags.Length : 0;
                for (int i = 0; i < length; i++)
                {
                    if(hittable.tag == tags[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 지정한 대상들만 타격 대상으로 간주하는 클래스
    /// </summary>
    public class TargetArea : Target
    {
        private IHittable[] hittables;

        public TargetArea(IHittable[] hittables)
        {
            this.hittables = hittables;
        }

        public override void Show()
        {
#if UNITY_EDITOR
            int length = hittables != null? hittables.Length : 0;
            for(int i = 0; i < length; i++)
            {
                //Vector2[] vectors = GetColliderEdges(hittables[i].GetCollider2D());
                //for (int j = 0; j < vectors.Length; j++)
                //{
                //    if (j > 0)
                //    {
                //        Debug.DrawLine(vectors[j - 1], vectors[j], Color.red, DrawDuration);
                //        if (j == vectors.Length - 1)
                //        {
                //            Debug.DrawLine(vectors[j], vectors[0], Color.red, DrawDuration);
                //        }
                //    }
                //}
            }
#endif
        }

        public override bool CanStrike(IHittable hittable)
        {
            if (hittable != null)
            {
                int length = hittables.Length;
                for (int i = 0; i < length; i++)
                {
                    if (hittable == hittables[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 특정 위치의 특정 태그 값을 가지면 타격 대상으로 간주하는 클래스
    /// </summary>
    public class PolygonArea : TagTarget
    {
        private Vector2 center;

        private Vector2[] points;

        public PolygonArea(Vector2 center, Vector2[] points, string[] tags) : base(tags)
        {
            this.center = center;
            this.points = points;
        }

        public override void Show()
        {
#if UNITY_EDITOR
            int length = points != null ? points.Length : 0;
            if (length > 0)
            {
                if (length > 1)
                {
                    for (int i = 1; i < length; i++)
                    {
                        if (center + points[i - 1] != center + points[i])
                        {
                            Debug.DrawLine(center + points[i - 1], center + points[i], Color.red, DrawDuration);
                        }
                        else
                        {
                            DrawDot(center + points[i], Color.red, DrawDuration);
                        }
                    }
                }
                else if (length > 0)
                {
                    DrawDot(center + points[0], Color.red, DrawDuration);
                }
            }
            else
            {
                DrawDot(center, Color.red, DrawDuration);
            }
#endif
        }

        public override bool CanStrike(IHittable hittable)
        {
            if (base.CanStrike(hittable) == true)
            {
                Collider2D collider2D = hittable.GetCollider2D();
                int length = points != null ? points.Length : 0;
                if (length > 0)
                {
                    Vector2[] polygon = new Vector2[length];
                    for(int i = 0; i < length; i++)
                    {
                        polygon[i] += center;
                        if (collider2D.OverlapPoint(polygon[i]))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return collider2D.OverlapPoint(center);
                }
            }
            return false;
        }
    }
}