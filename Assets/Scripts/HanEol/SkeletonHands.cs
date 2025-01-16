using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHands : MonoBehaviour
{
    [SerializeField]bool alive;
    [SerializeField]bool detectPlayer;
    [SerializeField]Animator animator;
    [SerializeField]Player player;
    [SerializeField] LayerMask playerMask;
    private Collider2D detectRangeCol;
    private Collider2D attackRangeCol;
    [SerializeField]int damage;
    [SerializeField]float detectRange;
    [SerializeField]float attackRange;
    [SerializeField]float responTime;
    [SerializeField]float attackTime;//공격에 걸리는 시간
    [SerializeField] float attackCoolTime;//공격 후 재시전 까지 걸리는 시간
    private Vector2 rotation = new Vector2(0, 0);//Rotation.y로 방향 조절

    float left = -180f;
    float right = 0f;


    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMask = LayerMask.GetMask("Player");
        
    }

    private void Update()
    {
        DetectPlayer();
    }


    void DetectPlayer()
    {
        detectRangeCol = Physics2D.OverlapCircle(transform.position, detectRange, playerMask);
        if(detectRangeCol != null)
        {
            detectPlayer = true;
            animator.SetBool("DetectPlayer",true);
        }
        
    }

}
