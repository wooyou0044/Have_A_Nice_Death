using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WomanGhostAi_Clone : Walker
{
    bool isspotted;
    [SerializeField]
    private Animator surprisedAnimator;

    [SerializeField]
    private AnimatorPlayer animatorPlayer;

    [SerializeField,Header("애니메이션 클립")]
    private AnimationClip idleClip;
    [SerializeField]
    private AnimationClip spotteddClip;
    [SerializeField]
    private AnimationClip attackClip;
    [SerializeField]
    private AnimationClip uturnClip;

    //적 감지
    public LayerMask playerLayermask;

    [SerializeField, Header("공격범위")] float ghostSightRange;
    Transform target;
    bool isturn;

    private bool movingRight = true;
    private Vector2 startPos;
    [SerializeField, Header("이동거리")] float moveDistance = 1f;
    
    
    private void Start()
    {
        startPos = transform.position;
        animatorPlayer = GetComponent<AnimatorPlayer>();
        surprisedAnimator = surprisedAnimator.gameObject.GetComponent<Animator>();                
    }

    private void Update()
    {
        Move();
        DetectPlayer();
    }

    void Move()
    {
        if (movingRight == true)
        {
            MoveRight();
            HandleRotation();
            if (transform.position.x > startPos.x + moveDistance)
            {
                animatorPlayer.Play(uturnClip, idleClip, true);
                movingRight = false;
            }
        }
        else
        {
            MoveLeft();
            HandleRotation();
            if (transform.position.x < startPos.x - moveDistance)
            {
                animatorPlayer.Play(uturnClip, idleClip, true);
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
            if(target == null)
            {
                animatorPlayer.Play(spotteddClip, true);
            }
            target = player.transform;


            ShowExclamationMark();
            SpottedPlayer();         
        }
        else if(target != null)
        {
            animatorPlayer.Play(idleClip, true);
            target = null;
        }
    }
    void SpottedPlayer()
    {        
        MoveStop();
        if (target.position.x <= transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);            
        }
    }    
    void ShowExclamationMark()
    {
        if (surprisedAnimator != null)
        {
            // 느낌표 활성화
            surprisedAnimator.gameObject.SetActive(true);
            Invoke(nameof(HideExclamationMark), 1f);

        }
    }
    void HideExclamationMark()
    {
        if (surprisedAnimator != null)
        {
            surprisedAnimator.gameObject.SetActive(false);
        }
    }
    private void WomanUturn()
    {        
        //Vector2 movement = movingRight ? Vector2.right : Vector2.left;        
        //if()
        //{
        //    animatorPlayer.Play(uturnClip, true);
        //}        
    }
    
}
