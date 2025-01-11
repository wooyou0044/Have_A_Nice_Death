using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ObjectPooler : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform {
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

    //ȿ�� ������Ʈ���� �����ϴ� ��ųʸ�
    private Dictionary<GameObject, List<GameObject>> _gameObjects = new Dictionary<GameObject, List<GameObject>>();

    //�̻��� ������Ʈ���� �����ϴ� ��ųʸ�
    private Dictionary<Projectile, List<Projectile>> _projectiles = new Dictionary<Projectile, List<Projectile>>();
       
    /// <summary>
    /// ���ӿ��� �Ͼ ȿ�� ����Ʈ���� Ǯ�����ִ� �޼���
    /// </summary>
    /// <param name="original"></param>
    /// <param name="position"></param>
    /// <param name="transform"></param>
    public void Set(GameObject original, Vector2 position, Transform transform = null)
    {
        if(original != null)
        {
            //�ش� �������� Ű ������ �����ϰ� �ִٸ�
            if(_gameObjects.ContainsKey(original) == true)
            {
                //Ű�� ����Ʈ ����� ��ȸ�ϸ鼭 ��Ȱ��ȭ �� ������Ʈ�� ã�´�.
                foreach(GameObject gameObject in _gameObjects[original])
                {
                    if(gameObject.activeInHierarchy == false)
                    {
                        gameObject.transform.position = position;
                        if(transform != null)
                        {
                            gameObject.transform.parent = transform;
                            gameObject.transform.rotation = transform.rotation;
                        }
                        else
                        {
                            gameObject.transform.parent = getTransform;
                            gameObject.transform.rotation = Quaternion.identity;
                        }
                        gameObject.SetActive(true);
                        return;
                    }
                }
                bool parent = transform != null;
                GameObject value = Instantiate(original, position, parent ? transform.rotation : Quaternion.identity, parent ? transform : getTransform);
                _gameObjects[original].Add(value);
            }
            //�װ� �ƴ϶�� Ű ���� �����ϰ� ��ü�� �����Ѵ�.
            else
            {
                bool parent = transform != null;
                GameObject value = Instantiate(original, position, parent ? transform.rotation: Quaternion.identity, parent ? transform: getTransform);
                _gameObjects.Add(original, new List<GameObject>() { value });
            }
        }
    }

    public void Set(Projectile original, Vector2 position, Transform transform = null)
    {
        if (original != null)
        {
            if(_projectiles.ContainsKey(original) == true)
            {

            }
            else
            {
                bool parent = transform != null;
                Projectile value = Instantiate(original, position, parent ? transform.rotation : Quaternion.identity, parent ? transform : getTransform);
                _projectiles.Add(original, new List<Projectile>() { value });
            }
        }
    }
}