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

    [SerializeField, Header("�߻�ü ������")]
    private bool _adhesion = false;
    [SerializeField, Header("�߻�ü �����")]
    private bool _penetration = false;

    [SerializeField, Header("�߻� ���� �ð�"),Range(0, byte.MaxValue)]
    private float _dealyTime = 0;
    [SerializeField, Header("���ư��� �ð�"), Range(0, byte.MaxValue)]
    private float _flyingTime = 1;
    [SerializeField, Header("���ư��� �ӵ�"), Range(0, byte.MaxValue)]
    private float _flyingSpeed = 1;
    [SerializeField, Header("���� ���")]
    private Shape _shape = null;
    [SerializeField, Header("�´� ��� ��ο��� ��Ÿ�� ȿ�� ������Ʈ")]
    private GameObject _hitObject = null;
    [SerializeField, Header("�ش� ��ü�� �Ҹ��ϸ鼭 ��Ÿ�� ȿ�� ������Ʈ")]
    private GameObject _explosionObject = null;

    private IEnumerator _coroutine = null;

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
        if(_shape != null && Application.isPlaying == false)
        {
            Strike.PolygonArea area = _shape.GetPolygonArea(getTransform, null);
            area.Show(_gizmoColor);
        }
    }

#endif

    [SerializeField]
    private Strike _strike;
    [SerializeField]
    private string[] _tags;

    private Action<Strike, Strike.Area, GameObject> _action1 = null;
    private Action<GameObject, Vector2, Transform> _action2 = null;

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

    /// <summary>
    /// Ư�� ���⿡�� �߻�ü�� �� ����ϴ� �Լ�
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="action1"></param>
    /// <param name="action2"></param>
    private void Shot(Vector2 position, Quaternion rotation, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2)
    {
        getTransform.position = position;
        getTransform.rotation = rotation;
        _action1 = action2;
        _action2 = action1;
        //StopAllCoroutines();
        //StartCoroutine(DoProject());
        //IEnumerator DoProject()
        //{
        //    yield return new WaitForSeconds(_dealyTime);
        //    float duration = _flyingTime;
        //    while (duration >= 0 || _flyingTime == 0)
        //    {
        //        float deltaTime = Time.deltaTime;
        //        Vector2 direction = getTransform.right.normalized;
        //        getTransform.Translate(direction * _flyingSpeed * deltaTime, Space.World);
        //        duration -= deltaTime;
        //        yield return null;
        //    }
        //    Explode();
        //}
    }

    public void Shot(Transform user, IHittable target, Action<GameObject, Vector2, Transform> action1, Action<Strike, Strike.Area, GameObject> action2)
    {
        if (user != null)
        {
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);
            }
            if (_adhesion == true)
            {
                getTransform.parent = user;
            }
            if (target != null)
            {
                Shot(user.position, Quaternion.LookRotation((target.transform.position - user.position).normalized), action1, action2);
            }
            else
            {
                Shot(user.position, user.rotation, action1, action2);
            }
        }
        else if (target != null)
        {
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);
            }
            if (_adhesion == true)
            {
                getTransform.parent = target.transform;
            }
            Shot(target.transform.position, target.transform.rotation, action1, action2);
        }
        else if(gameObject.activeInHierarchy == true)
        {
            gameObject.SetActive(false);
        }
    }
}