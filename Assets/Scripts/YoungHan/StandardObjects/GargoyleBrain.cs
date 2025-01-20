using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BossMovement))]
public class GargoyleBrain : MonoBehaviour
{
    [SerializeField, Header("��ų �غ� �ð�"), Range(0, byte.MaxValue)]
    private float _preparationTime = 2f;

    private enum Skill
    {
        Scratching, //������
        Dash,       //����
        Stone,      //�� ������
        Fall,       //����
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Trace(IHittable hittable)
    {
        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            while (true)
            {
                yield return new WaitForSeconds(2);
                yield return null;
            }
        }
    }
}
