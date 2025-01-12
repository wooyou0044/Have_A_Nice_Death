using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� Ÿ���ϴ� ����ü
/// </summary>
[Serializable]
public struct Strike
{
    //Ÿ���ϴ� ����
    [SerializeField, Header("����� ȸ��, ������ ����")]
    private int power;

    //���� Ÿ�� �뵵�� �����̶�� ���ݷ��� Ȯ���ϴ� �뵵
    [SerializeField, Header("�뵵�� ���� �� ���ط��� ")]
    private byte extension;

    //���������� ��󿡰� ������ ������� ��ȯ�Ѵ�.
    public int result
    {
        get
        {
            //power�� �����̸� ������ ���� �뵵
            if (power < 0)
            {
                return power - UnityEngine.Random.Range(0, extension);
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

        protected static void DrawDot(Vector2 point, Color color, float duration = 0)
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
    public class TagTarget : Area
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
    public class PolygonArea : TagTarget
    {
        private Vector2[] vertices;

        public PolygonArea(Vector2[] vertices, string[] tags) : base(tags)
        {
            this.vertices = vertices;
        }

        public override void Show()
        {
#if UNITY_EDITOR
            int length = vertices != null ? vertices.Length : 0;
            if(length > 1)
            {
                for (int i = 0; i < length - 1; i++)
                {
                    Debug.DrawLine(vertices[i], vertices[i+ 1], Color.red);
                }
            }
            else if(length > 0)
            {
                DrawDot(vertices[0], Color.red);
            }
#endif
        }

        public override bool CanStrike(IHittable hittable)
        {
            if (base.CanStrike(hittable) == true)
            {
                Collider2D collider2D = hittable.GetCollider2D();
                if(collider2D != null)
                {
                    int length = vertices != null ? vertices.Length : 0;
                    for (int i = 0; i < length; i++)
                    {
                        if (collider2D.OverlapPoint(vertices[i]) == true)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}