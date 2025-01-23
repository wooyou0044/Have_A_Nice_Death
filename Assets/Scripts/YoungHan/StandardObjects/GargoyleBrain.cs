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
    public float _rechargeTime = 3f;
    [SerializeField, Header("��ų ���� ����")]
    public Skill _skill = Skill.Dash;
    [SerializeField, Header("Ȱ�� ���� ����")]
    private float _leftBoundary;
    [SerializeField, Header("Ȱ�� ���� ������")]
    private float _rightBoundary;

    public enum Skill
    {
        Scratching, //������
        Dash,       //����
        Stone,      //�� ������
        Fall,       //����
    }

#if UNITY_EDITOR

    [SerializeField, Header("���� Ȯ�� ����")]
    private float _rayLength = 10f;

    private void OnDrawGizmos()
    {
        Debug.DrawRay(new Vector2(_leftBoundary, 0), Vector2.up * _rayLength, Color.red);
        Debug.DrawRay(new Vector2(_rightBoundary, 0), Vector2.up * _rayLength, Color.red);
        Debug.DrawRay(new Vector2(_leftBoundary, 0), Vector2.down * _rayLength, Color.red);
        Debug.DrawRay(new Vector2(_rightBoundary, 0), Vector2.down * _rayLength, Color.red);
    }

    private void OnValidate()
    {
        if (transform.position.x < _leftBoundary)
        {
            _leftBoundary = transform.position.x;
        }
        if (transform.position.x > _rightBoundary)
        {
            _rightBoundary = transform.position.x;
        }
    }
#endif

    private void OnEnable()
    {
        Trace(FindObjectOfType<Player>());
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
                        if ((hittable.transform.position.x < transform.position.x && transform.eulerAngles.y == 0) ||
                       (transform.position.x < hittable.transform.position.x && transform.eulerAngles.y == 180))
                        {
                            getBossMovement.UTurn();
                            yield return new WaitForSeconds(0.5f);
                        }
                        getBossMovement.FollowPlayer(hittable.transform.position.x);
                        //if((transform.eulerAngles.y == 180 && hittable.transform.position.x < transform.position.x) ||
                        //    (transform.eulerAngles.y == 0 && transform.position.x ))


                        getBossMovement.MoveStop();
                        getBossMovement.ComboAttack1();
                        break;
                    case Skill.Dash:
                        if((hittable.transform.position.x < transform.position.x && transform.eulerAngles.y == 0) ||
                            (transform.position.x < hittable.transform.position.x && transform.eulerAngles.y == 180))
                        {
                            getBossMovement.UTurn();
                            yield return new WaitForSeconds(0.5f);
                        }
                        getBossMovement.FlyMove();  //�������� �̵��ϴ� �Լ� �ʿ�
                        getBossMovement.AdjustRotation();
                        Collider2D collider2D = getBossMovement.GetCollider2D();
                        while ((transform.eulerAngles.y == 0 && collider2D.bounds.max.x < _rightBoundary) ||
                           (transform.eulerAngles.y == 180 && collider2D.bounds.min.x > _leftBoundary))
                        {
                            yield return null;
                        }
                        getBossMovement.UTurn();
                        yield return new WaitForSeconds(0.5f);
                        getBossMovement.DropDiagonalMove(); //�缱 ����
                        getBossMovement.AdjustRotation();
                        getBossMovement.UseDashSkill(3.059f);
                        while (getBossMovement.isGrounded == false)  //���� �� �� ���� ��ٸ�
                        {
                            yield return null;
                        }
                        getBossMovement.UTurn();
                        yield return new WaitForSeconds(0.5f);
                        getBossMovement.MoveOppositeEndPoint(); //���� ������ �� �ݴ�������� ����
                        getBossMovement.UseDashSkill(3.82f);
                        while (_leftBoundary < collider2D.bounds.min.x && _rightBoundary > collider2D.bounds.max.x) //Ư�� ��ġ�� ������ �� ���� ��ٸ�
                        {
                            yield return null;
                        }
                        getBossMovement.UTurn();
                        yield return new WaitForSeconds(0.2f);
                        getBossMovement.MoveStop();
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