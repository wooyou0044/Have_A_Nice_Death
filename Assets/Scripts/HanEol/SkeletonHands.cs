using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class SkeletonHands : MonoBehaviour, IHittable
{
    [SerializeField]bool alive = true;
    [SerializeField]bool detectPlayer;
    [SerializeField]Animator animator;
    [SerializeField] LayerMask playerMask;
    private Collider2D detectRangeCol;
    private Collider2D attackRangeCol;
    [SerializeField]private bool attackAvailale;
    [SerializeField]int damage;
    [SerializeField]float detectRange;
    [SerializeField]float attackRange;
    [SerializeField]float responTime = 5f;
    [SerializeField]float attackTime;//공격에 걸리는 시간
    [SerializeField] float attackCoolTime;//공격 후 재시전 까지 걸리는 시간
    private Quaternion rotation = new Quaternion(0,0,0,0);//Rotation.y로 방향 조절
    [SerializeField] IEnumerator currentCoroutine;
    Strike strike;
    [SerializeField]private Transform imageTransform;
    float left = 0;
    float right =-180f;
    private Collider2D thisCol;//IHittable 규격을 위한 자신의 콜라이더
    [SerializeField]private Skill skill;

    //미구현 항목: 방향 틀기, 데미지 주기 / Fix: 잘 안맞음 이거
    public bool isAlive
    {
        get { return alive; }
        private set { alive = value; }
    }

   

    public void Hit(Strike strike)
    {
        if (isAlive == true)
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = IamDead();
            StartCoroutine(currentCoroutine);
        }
    }



    public Collider2D GetCollider2D()
    {
        return thisCol;
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerMask = LayerMask.GetMask("Player");
        alive = true;
        thisCol = GetComponent<Collider2D>();
        imageTransform = transform.GetChild(0);
    }

    private void Update()
    {
        if(alive == true)
        {
            DetectPlayer();
            if (detectPlayer == true)
            {
                attackCoolTime -= Time.deltaTime;
            }
        }
       
    }

    /// <summary>
    /// 플레이어 감지 + 공격 함수
    /// </summary>
    void DetectPlayer()
    {
        //플레이어 감지 범위
        detectRangeCol = Physics2D.OverlapCircle(transform.position, detectRange, playerMask);
       
        //플레이어를 감지했다!
        if(detectRangeCol != null)
        {
            detectPlayer = true;
            animator.SetBool("DetectPlayer",true);
            float calPosition = detectRangeCol.transform.position.x - transform.position.x;
            //플레이어가 바라보는 방향으로 바꾸기. 그런데 이제 역순
            if (calPosition < 0)
            {
                rotation.y = right;
                transform.rotation = rotation;
            }
            else
            {
                rotation.y = left;
                transform.rotation = rotation;
            }

            //공격 범위 설정
            attackRangeCol = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
            //공격 범위에 플레이어가 들어와따?
            if (attackRangeCol != null)
            {
                attackAvailale = true;
                if (currentCoroutine == null)
                {
                    currentCoroutine = Attack();
                    StartCoroutine(currentCoroutine);
                }
                
            }
            else
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                    animator.SetBool("Attack", false);
                }
                attackRangeCol = null;
                currentCoroutine = null;
                attackAvailale = false;
            }
        }
        else
        {
            if(detectPlayer != false) detectPlayer = false;
            animator.SetBool("DetectPlayer", false);
            attackCoolTime = 0f;

        }


        
       


    }

    IEnumerator Attack()
    {
        
        while(attackAvailale == true)
        {
            if(attackCoolTime < 0)
            {

                animator.SetBool("Attack", true);

                yield return new WaitForSeconds(0.72f);
                if (detectRangeCol != null)
                {
                    IHittable hittable = detectRangeCol.transform.GetComponent<IHittable>();
                    skill.Use(transform, hittable, new string[] {"Player"}, GameManager.ShowEffect, GameManager.Use, GameManager.GetProjectile);
                    Debug.Log("때렸다!");
                }
                yield return new WaitForSeconds(0.28f);
                animator.SetBool("Attack", false);
                attackCoolTime = 2;
            }
            yield return null;

        }
    }

    IEnumerator IamDead()
    {
        Debug.Log("죽어라!!");
        alive = false;
        animator.SetTrigger("Dead");
        animator.SetBool("Regen", true);
        imageTransform.position = transform.position;
        yield return new WaitForSecondsRealtime(responTime);
        animator.SetBool("Regen", false);
        alive = true;
        currentCoroutine = null;
    }

}
