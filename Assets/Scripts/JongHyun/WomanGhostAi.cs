using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WomanGhostAi : Walker
{
    bool isspotted;

    [SerializeField, Header("놀람애니메이션")]
    Animator surprisedAnimator;

    Animator ghostAnimator;
    //적 감지
    public LayerMask playerLayermask;
    
    [SerializeField, Header("공격범위")]float ghostSightRange;
    Transform target;
    bool isAttacking;

    private bool movingRight = true;
    private Vector2 startPos;    
    [SerializeField, Header("이동거리")] float moveDistance = 1f;

    

    private void Start()
    {
        startPos = transform.position;        
        ghostAnimator = GetComponent<Animator>();
        
        if (surprisedAnimator != null)
        {
            surprisedAnimator.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        Move();
        WomanWalk();        
        DetectPlayer();
    }

    void Move()
    {
        if(movingRight== true)
        {
            MoveRight();
            HandleRotation();
            if (transform.position.x > startPos.x + moveDistance)
            {
                movingRight = false;
            }
        }        
        else
        {
            MoveLeft();
            HandleRotation();
            if (transform.position.x < startPos.x - moveDistance)
            {
                movingRight = true;
            }
        }
    }
    
    private void HandleRotation()
    {
        if (movingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void DetectPlayer()
    {        
        Collider2D player;
        player = Physics2D.OverlapCircle(transform.position, ghostSightRange, playerLayermask);        
        if (player != null)
        {            
            target = player.transform;
           
            SpottedPlayer();
            PlaySurprisedAnimation();
            ShowExclamationMark();
            if (player != null)
            {
                target = player.transform;
                AttackPlayer();
            }            
        }
    }
    void SpottedPlayer()
    {
        MoveStop();
        ghostAnimator.SetTrigger("IsSpotted");
        ghostAnimator.SetBool("IsWalk", false);       
        
        if (target.position.x <= transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    void PlaySurprisedAnimation()
    {
        //if (surprisedAnimator != null)
        //{
        //    surprisedAnimator.SetTrigger("Surprised");
        //}
    }
    void ShowExclamationMark()
    {
        if (surprisedAnimator != null)
        {
            // 느낌표 활성화
            surprisedAnimator.gameObject.SetActive(true);

            Invoke(nameof(HideExclamationMark), 2f);
        }
    }
    void HideExclamationMark()
    {
        if (surprisedAnimator != null)
        {
            surprisedAnimator.gameObject.SetActive(false);
        }
    }
    private void WomanWalk()
    {
        Vector2 movement = movingRight ? Vector2.right : Vector2.left;
        if (ghostAnimator != null)
        {
            ghostAnimator.SetBool("IsWalk", true);
        }
    }
    void AttackPlayer()
    {
        MoveStop();
        ghostAnimator.SetTrigger("IsSpotted");
        ghostAnimator.SetBool("IsAttack", true);
        ghostAnimator.SetBool("IsWalk", false);
        
    }

}
