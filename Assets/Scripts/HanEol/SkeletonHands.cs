using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class SkeletonHands : MonoBehaviour, IHittable
{
    [SerializeField]bool alive = true;
    [SerializeField]bool detectPlayer;
    [SerializeField]Animator animator;
   // [SerializeField]Player player;
    [SerializeField] LayerMask playerMask;
    private Collider2D detectRangeCol;
    private Collider2D attackRangeCol;
    [SerializeField]private bool attackAvailale;
    [SerializeField]int damage;
    [SerializeField]float detectRange;
    [SerializeField]float attackRange;
    [SerializeField]float responTime;
    [SerializeField]float attackTime;//공격에 걸리는 시간
    [SerializeField] float attackCoolTime;//공격 후 재시전 까지 걸리는 시간
    private Vector2 rotation = new Vector2(0, 0);//Rotation.y로 방향 조절
    [SerializeField] IEnumerator currentCoroutine;
    Strike strike;
    
    float left = -180f;
    float right = 0f;
    private Collider2D thisCol;//IHittable 규격을 위한 자신의 콜라이더

    public bool isAlive
    {
        get { return isAlive; }
        private set { isAlive = value; }
    }

    public string tag
    {
        get { return gameObject.tag; }
        set { gameObject.tag = value; }
    }

    public Transform transform
    {
        get { return gameObject.transform; }
    }

    public void Hit(Strike strike)
    {
        if(isAlive == true)
        {

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
                yield return new WaitForSeconds(1);
             
                animator.SetBool("Attack", false);
                attackCoolTime = 2;
            }
            yield return null;

        }
    }

    IEnumerator IamDead()
    {
        alive = false;
        animator.SetTrigger("Dead");
        animator.SetBool("Regen", true);
        yield return new WaitForSecondsRealtime(responTime);
        alive = true;
        animator.SetBool("Regen", false);
    }

}
