using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class YoungHan : MonoBehaviour, IHittable, ILootable
{
    [SerializeField]
    private MonoBehaviour _prefab;

    public bool isAlive
    {
        get
        {
            return false;
        }
    }

    private bool _hasCollider2D = false;

    private Collider2D _collider2D = null;

    private Collider2D getCollider2D
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
    public void Hit(Strike strike)
    {
        GameManager.Report(this, strike.result);
    }

    public Collider2D GetCollider2D()
    {
        return getCollider2D;
    }

    public MonoBehaviour GetLootObject()
    {
        throw new System.NotImplementedException();
    }

}
