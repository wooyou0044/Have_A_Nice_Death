using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    private bool hasTransform = false;
    private Transform _transform = null;
    private Transform getTransform {
        get
        {
            if (hasTransform == false)
            {
                _transform = transform;
                hasTransform = true;
            }
            return _transform;
        }
    }

    private bool _hasCollider2D = false;

    private Collider2D _collider2D = null;

    private Collider2D getCollider2D {
        get
        {
            if (_hasCollider2D == false)
            {
                _hasCollider2D = true;
                _collider2D = GetComponent<Collider2D>();
            }
            return _collider2D;
        }
    }

    [SerializeField, Header("�߻� ���� �ð�"),Range(0, byte.MaxValue)]
    private float _dealyTime = 0;
    [SerializeField, Header("���ư��� �ð�"), Range(0, byte.MaxValue)]
    private float _flyingTime = 1;
    [SerializeField, Header("���ư��� �ӵ�"), Range(0, byte.MaxValue)]
    private float _flyingSpeed = 1;
    [SerializeField, Header("���� ��ȿ �Ÿ�"), Range(0, byte.MaxValue)]
    private float _strikeRange = 2;
    [SerializeField, Header("�´� ��� ��ο��� ��Ÿ�� ȿ�� ������Ʈ")]
    private GameObject _hitObject = null;
    [SerializeField, Header("�ش� ��ü�� �Ҹ��ϸ鼭 ��Ÿ�� ȿ�� ������Ʈ")]
    private GameObject _explosionObject = null;
    [SerializeField, Header("���� ���")]
    private Shape _shape = null;

#if UNITY_EDITOR
    [SerializeField, Header("���� ��ȿ �Ÿ� ǥ�� �ð�"), Range(0, byte.MaxValue)]
    private float _gizmoDuration = 1;
    [SerializeField, Header("���� ��ȿ �Ÿ� ǥ�� ����")]
    private Color _gizmoColor = Color.red;

    /// <summary>
    /// Ÿ�� ��ȿ �Ÿ��� ������ ��Ÿ�ӿ� ǥ���ϴ� �Լ�
    /// </summary>
    private void OnDrawGizmos()
    {
        if(Application.isPlaying == false)
        {
            DrawStrikeRange();
        }
    }

    private void DrawStrikeRange()
    {
        int segments = 32; // ���� �ٻ��� ���׸�Ʈ ��
        float radius = _strikeRange * 0.5f;
        Vector2 offset = getTransform.position;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * Mathf.PI * 2 / segments;
            float angle2 = (i + 1) * Mathf.PI * 2 / segments;
            Vector2 point1 = new Vector2(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius) + offset;
            Vector2 point2 = new Vector2(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius) + offset;
            Debug.DrawLine(point1, point2, _gizmoColor);
        }
    }
#endif

    private Strike _strike;
    private string[] _tags;
    private Action<Strike, Strike.Area, GameObject> _action1 = null;
    private Action<GameObject, Vector2> _action2 = null;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Explode();
    }

    private void Explode()
    {
        StopAllCoroutines();
        Vector2 position = getTransform.position;
        //_action1?.Invoke(_strike, new Strike.PolygonArea(position, null, _tags), _hitObject);
        //_action2?.Invoke(_explosionObject, position);
    }

    private void Shot(Vector2 position, Quaternion rotation, Strike strike, string[] tags, Action<Strike, Strike.Area, GameObject> action1, Action<GameObject, Vector2> action2)
    {
        gameObject.SetActive(true);
        getCollider2D.enabled = true;
        getTransform.position = position;
        getTransform.rotation = rotation;
        _strike = strike;
        _tags = tags;
        _action1 = action1;
        _action2 = action2;
        StopAllCoroutines();
        StartCoroutine(DoProject());
        IEnumerator DoProject()
        {
            yield return new WaitForSeconds(_dealyTime);
            float duration = _flyingTime;
            while (duration >= 0 || _flyingTime == 0)
            {
                float deltaTime = Time.deltaTime;
                Vector2 direction = getTransform.right.normalized;
                getTransform.Translate(direction * _flyingSpeed * deltaTime, Space.World);
                duration -= deltaTime;
                yield return null;
            }
            Explode();
        }
    }

    /// <summary>
    /// Ư�� ��ġ�� �����Ͽ� �߻�ü�� �� ����ϴ� �Լ�
    /// </summary>
    /// <param name="strike"></param>
    /// <param name="start"></param>
    /// <param name="target"></param>
    /// <param name="tags"></param>
    /// <param name="action1"></param>
    /// <param name="action2"></param>
    public void Shot(Strike strike, Vector2 start, Vector2 target, string[] tags, Action<Strike, Strike.Area, GameObject> action1, Action<GameObject, Vector2> action2)
    {
        Shot(start, Quaternion.LookRotation((target - start).normalized), strike, tags, action1, action2);
    }

    /// <summary>
    ///  Ư�� ���⿡�� �߻�ü�� �� ����ϴ� �Լ�
    /// </summary>
    /// <param name="strike"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="tags"></param>
    /// <param name="action1"></param>
    /// <param name="action2"></param>
    public void Shot(Strike strike, Vector2 position, Quaternion rotation, string[] tags, Action<Strike, Strike.Area, GameObject> action1, Action<GameObject, Vector2> action2)
    {
        Shot(position, rotation, strike, tags, action1, action2);
    }
}