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
    [SerializeField]float attackTime;//���ݿ� �ɸ��� �ð�
    [SerializeField] float attackCoolTime;//���� �� ����� ���� �ɸ��� �ð�
    private Vector2 rotation = new Vector2(0, 0);//Rotation.y�� ���� ����
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

       if(attackRangeCol == null)
        {
            attackRangeCol = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
        }
         
        

        if (attackRangeCol != null)
        {
            attackAvailale = true;
            if(currentCoroutine == null) currentCoroutine = Attack();
            StartCoroutine(currentCoroutine);
            
            
        }
        else
        {
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            attackAvailale = false;
            attackRangeCol = null;
        }


    }

    IEnumerator Attack()
    {
        
        while(attackAvailale == true)
        {
            if(attackCoolTime < 0)
            {
               
                animator.SetBool("Attack", true);
                Debug.Log($"Animator Attack Parameter: {animator.GetBool("Attack")}");
                Debug.Log($"Current State: {animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")}");
                yield return null;
                
                animator.SetBool("Attack", false);
                Debug.Log("����");
                attackCoolTime = 2;
            }
            yield return new WaitForSeconds(attackCoolTime);

        }
    }

}
