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
                Skill skill = Skill.Scratching;
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
                switch(skill)
                {
                    case Skill.Scratching:
                        getBossMovement.MoveToAttack(hittable);
                        break;
                    case Skill.Dash:
                        break;
                    case Skill.Stone:
                        break;
                    case Skill.Fall:
                        break;
                }
                yield return new WaitForSeconds(5f);
            }
        }
    }
}