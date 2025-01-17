using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHands : MonoBehaviour
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

    float left = -180f;
    float right = 0f;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerMask = LayerMask.GetMask("Player");
        alive = true;
        
    }

    private void Update()
    {
        DetectPlayer();
        if (detectPlayer == true && alive == true)
        {
            attackCoolTime -= Time.deltaTime;
        }
    }


    void DetectPlayer()
    {
        detectRangeCol = Physics2D.OverlapCircle(transform.position, detectRange, playerMask);
        if(detectRangeCol != null)
        {
            detectPlayer = true;
            animator.SetBool("DetectPlayer",true);
        }
        else
        {
            if(detectPlayer != false) detectPlayer = false;
            animator.SetBool("DetectPlayer", false);
            attackCoolTime = 0f;

        }


        
        attackRangeCol = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
        
         
        

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

}
