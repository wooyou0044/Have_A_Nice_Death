using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� Ÿ���ϴ� ����ü
/// </summary>
public struct Strike
{
    //Ÿ���ϴ� ����
    private int power;

    //���� Ÿ�� �뵵�� �����̶�� ���ݷ��� Ȯ���ϴ� �뵵
    private byte extension;

    //���������� ��󿡰� ������ ������� ��ȯ�Ѵ�.
    public int result
    {
        get
        {
            //power�� �����̸� ������ ���� �뵵
            if (power < 0)
            {
                return power - Random.Range(0, extension);
            }
            //power�� ����̸� ȸ���� ��Ű�� ���� �뵵
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
    /// �ش� ����� Ÿ�� ������� �����ϴ� Ŭ����(�� �� ���� null�̶�� ����� �������� �ʰ� ���� Ÿ��)
    /// </summary>
    public abstract class Area
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
    /// Ư�� �±׸� Ÿ�� ������� �����ϴ� Ŭ����
    /// </summary>
    public class TagArea : Area
    {
        private string[] tags;

        public TagArea(string[] tags)
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
                    Debug.Log($"<color=red>Ÿ�� ���:{tags[i]}</color>");
                }
            }
            else
            {
                Debug.Log($"<color=black>Ÿ�� ��� ����</color>");
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
    /// ������ ���鸸 Ÿ�� ������� �����ϴ� Ŭ����
    /// </summary>
    public class TargetArea : Area
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
    /// Ư�� ��ġ�� Ư�� �±� ���� ������ Ÿ�� ������� �����ϴ� Ŭ����
    /// </summary>
    public class PolygonArea : TagArea
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