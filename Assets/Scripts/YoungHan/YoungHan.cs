using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class YoungHan :MonoBehaviour, IHittable
{
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

    [SerializeField]
    private int hitCount = 0;


    public bool isAlive {
        get
        {
            return true;
        }
    }

    public Collider2D GetCollider2D()
    {
        return getCollider2D;
    }

    public void Hit(Strike strike)
    {
        int result = strike.result;
        if(result < 0)
        {
            hitCount++;
        }
    }
}
