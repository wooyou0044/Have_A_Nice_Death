using UnityEngine;

[DisallowMultipleComponent]
public class YoungHan :MonoBehaviour
{
    private bool _hasRigidbody2D = false;

    private Rigidbody2D _rigidbody2D = null;

    private Rigidbody2D getRigidbody2D
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

    [SerializeField]
    private Vector2 point;

    private void OnDrawGizmos()
    {
        Debug.DrawRay(point, (Vector2)transform.position - point);
    }

    private void Start()
    {
        Vector2 direction = (Vector2)transform.position - point;
        getRigidbody2D.velocity -= direction;
    }
}
