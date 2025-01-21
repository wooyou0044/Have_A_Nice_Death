using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BossMovement))]
public class GargoyleBrain : MonoBehaviour
{
    private bool _hasBossMovement = false;

    private BossMovement _bossMovement = null;

    private BossMovement getBossMovement
    {
        get
        {
            if(_hasBossMovement == false)
            {
                _hasBossMovement = true;
                _bossMovement = GetComponent<BossMovement>();
            }
            return _bossMovement;
        }
    }

    [SerializeField, Header("��ų �غ� �ð�"), Range(0, byte.MaxValue)]
    private float _preparationTime = 2f;
    [SerializeField, Header("��ų �޽� �ð�"), Range(0, byte.MaxValue)]
    private float _rechargeTime = 3f;
    [SerializeField, Header("��ų ���� ����")]
    private Skill _skill = Skill.Dash;

    private enum Skill
    {
        Scratching, //������
        Dash,       //����
        Stone,      //�� ������
        Fall,       //����
    }

    private void OnEnable()
    {
        Player player = FindObjectOfType<Player>();
        Trace(player);
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
            while (getBossMovement.isAlive == true)
            {
                yield return new WaitForSeconds(_preparationTime);
                //float probability = Random.Range(0, 100);
                //if (probability < 50)
                //{
                //    skill = Skill.Dash;
                //}
                //else if(probability < 30)
                //{
                //    skill = Skill.Stone;
                //}
                //else if (probability < 10)
                //{
                //    skill = Skill.Fall;
                //}
                switch(_skill)
                {
                    case Skill.Scratching:                     
                        //�޺�1 ���� �Լ� �ʿ�
                        //����� �������� �˷��ִ� �Լ� �ʿ�
                        //�޺�2 ���� �Լ� �ʿ�
                        break;
                    case Skill.Dash:   
                        //�������� �̵��ϴ� �Լ� �ʿ�
                        //Ư�� ��ġ�� ������ �� ���� ��ٸ�
                        //getBossMovement.MoveToAttack(hittable); //�밢�� ����
                        while (getBossMovement.isGrounded == false)  //���� �� �� ����
                        {
                            yield return null;
                        }
                        //���� ������ �� �뽬�ϴ� �Լ� �ʿ�
                        break;
                    case Skill.Stone:
                        break;
                    case Skill.Fall:
                        break;
                }
                yield return new WaitForSeconds(_rechargeTime);
            }
        }
    }
}