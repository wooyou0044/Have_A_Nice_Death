using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ġ�� ������ �� �ְ� �̵��� �� �ִ� ��Ŀ Ŭ����
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Walker : MonoBehaviour, IPositionable, IMovable
{
    protected static readonly Vector2 LeftRotation = new Vector2(0, 180);
    protected static readonly Vector2 RightRotation = new Vector2(0, 0);

    private bool _hasTransform = false;

    private Transform _transform = null;

    protected Transform getTransform
    {
        get
        {
            if (_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    private bool _hasRigidbody2D = false;

    private Rigidbody2D _rigidbody2D = null;

    protected Rigidbody2D getRigidbody2D
    {
        get
        {
            if (_hasRigidbody2D == false)
            {
                _hasRigidbody2D = true;
                _rigidbody2D = GetComponent<Rigidbody2D>();
                _rigidbody2D.freezeRotation = true;
            }
            return _rigidbody2D;
        }
    }

    private bool _hasCollider2D = false;

    private Collider2D _collider2D = null;

    protected Collider2D getCollider2D
    {
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

    private List<Collision2D> _leftCollision2D = new List<Collision2D>();

    private List<Collision2D> _rightCollision2D = new List<Collision2D>();

    //��ü�� ��ġ���� �̵���Ű�ų� ��ȯ�ϴ� ������Ƽ
    public Vector2 position
    {
        get
        {
            return getTransform.position;
        }
        set
        {
            getTransform.position = value;
        }
    }

    //�̵� �ӵ�
    [SerializeField, Header("�̵� �ӵ�"), Range(0, float.MaxValue)]
    protected float _movingSpeed = 10;

    //���� �����ߴ��� ������ �Ǵ��ϴ� ������Ƽ
    [SerializeField]
    private bool _isGrounded = false;

    public bool isGrounded
    {
        get
        {
            return _isGrounded;
        }
    }

#if UNITY_EDITOR

    //����� ǥ�� ����
    [SerializeField, Header("����� ǥ�� ����")]
    private Color _gizmoColor = Color.black;

    /// <summary>
    /// ����Ƽ �����Ϳ��� �ٴ� �浹�� �����ϴ� ������ ������ �׾��ش�.
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        Bounds bounds = getCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
        float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
        float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
        float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
        Debug.DrawLine(new Vector2(minX, centerY), new Vector2(maxX, centerY), _gizmoColor);
        Debug.DrawLine(new Vector2(minX, centerY), new Vector2(minX, bounds.min.y), _gizmoColor);
        Debug.DrawLine(new Vector2(maxX, centerY), new Vector2(maxX, bounds.min.y), _gizmoColor);
        Debug.DrawLine(new Vector2(minX, bounds.min.y), new Vector2(maxX, bounds.min.y), _gizmoColor);
    }
#endif

    /// <summary>
    /// �ٴڿ� �浹�ϴ��� ���θ� Ȯ���ϴ� �޼���
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Bounds bounds = getCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
        float minX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.75f));
        float maxX = bounds.center.x + (radius * Mathf.Cos(Mathf.PI * -0.25f));
        float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 point = collision.contacts[i].point;
            if (point.x > minX && point.x < maxX && point.y < centerY)
            {
                _isGrounded = true;
            }
        }
    }

    /// <summary>
    /// ���ʿ� �浹ü�� �ִ��� ���θ� Ȯ���ϴ� �޼���
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (_isGrounded == false && getRigidbody2D.velocity.y == 0 && (_leftCollision2D.Count > 0 || _rightCollision2D.Count > 0))
        {
            _isGrounded = true;
        }
        Bounds bounds = getCollider2D.bounds;
        float radius = bounds.size.x * 0.5f;
        float centerY = bounds.min.y + radius + (radius * Mathf.Sin(Mathf.PI * -0.25f));
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 point = collision.contacts[i].point;
            if (point.y >= centerY)
            {
                if (bounds.min.x - IMovable.OverlappingDistance < point.x && point.x < bounds.center.x && _leftCollision2D.Contains(collision) == false)
                {
                    _leftCollision2D.Add(collision);
                }
                if (bounds.center.x < point.x && point.x < bounds.max.x + IMovable.OverlappingDistance && _rightCollision2D.Contains(collision) == false)
                {
                    _rightCollision2D.Add(collision);
                }
            }
        }
    }

    /// <summary>
    /// ���ϳ� ���� �浹ü�� ���������� Ȯ���ϴ� �޼���
    /// </summary>
    /// <param name="collision"></param>
    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (getRigidbody2D.velocity.y < IMovable.MinimumDropVelocity && _isGrounded == true)
        {
            _isGrounded = false;
        }
        _leftCollision2D.Remove(collision);
        _rightCollision2D.Remove(collision);
    }

    /// <summary>
    /// ���������� �̵� ��Ű�� �޼���
    /// </summary>
    public virtual void MoveRight()
    {
        if (_rightCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(+_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    /// <summary>
    /// �������� �̵� ��Ű�� �޼���
    /// </summary>
    public virtual void MoveLeft()
    {
        if (_leftCollision2D.Count == 0)
        {
            getRigidbody2D.velocity = new Vector2(-_movingSpeed, getRigidbody2D.velocity.y);
        }
    }

    /// <summary>
    /// �̵��� ���� ��Ű�� �޼���
    /// </summary>
    public virtual void MoveStop()
    {
        getRigidbody2D.velocity = new Vector2(0, getRigidbody2D.velocity.y);
    }
}