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

    [SerializeField, Header("스킬 준비 시간"), Range(0, byte.MaxValue)]
    private float _preparationTime = 2f;
    [SerializeField, Header("스킬 휴식 시간"), Range(0, byte.MaxValue)]
    private float _rechargeTime = 3f;
    [SerializeField, Header("스킬 시전 종류")]
    private Skill _skill = Skill.Dash;

    private enum Skill
    {
        Scratching, //할퀴기
        Dash,       //돌진
        Stone,      //돌 던지기
        Fall,       //낙하
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
                        //콤보1 공격 함수 필요
                        //기술이 끝났음을 알려주는 함수 필요
                        //콤보2 공격 함수 필요
                        break;
                    case Skill.Dash:   
                        //공중으로 이동하는 함수 필요
                        //특정 위치에 도달할 때 까지 기다림
                        //getBossMovement.MoveToAttack(hittable); //대각선 공격
                        while (getBossMovement.isGrounded == false)  //땅에 갈 때 까지
                        {
                            yield return null;
                        }
                        //땅에 떨어진 후 대쉬하는 함수 필요
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