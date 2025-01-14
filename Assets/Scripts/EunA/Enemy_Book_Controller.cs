using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Book_AI : Walker
{
    float detectTime;
    int enemyHealth;
    Animator EnemyBookAnimator;

    public LayerMask playerLayerMask;
    public GameObject PaperPlaneMuzzle;
    public AnimationClip idleClip;
    public AnimationClip uturnClip;
    public AnimationClip findClip;
    public AnimationClip hitClip;
    public AnimationClip attackClip;
    public AnimationClip dieClip;

    float BookSightRange;

    bool isAttacking = false;

    float moveElapsedTime;
    float moveTime = 4.0f;
    float moveDistance;
    bool isFacingRight;
    bool isUturn;

    int facingRight;
    Collider2D Detectplayer;
    Rigidbody2D enemyRigid;
    Vector2 leftEnemyLocation;
    Vector2 nowEnemyLocation;
    Vector2 rightEnemyLocation;

    [SerializeField]
    AnimatorPlayer BookAnimatorPlayer;

    void Start()
    {
        enemyHealth = 15;
        EnemyBookAnimator = GetComponent<Animator>();
        enemyRigid = GetComponent<Rigidbody2D>();
        BookSightRange = 10;
        moveDistance = 4.0f;
        leftEnemyLocation = new Vector2(transform.position.x, transform.position.y);
        BookAnimatorPlayer = GetComponent<AnimatorPlayer>();
        isFacingRight = true;
        isUturn = true;
    }

    void Update()
    {
        DetectPlayer();

        if (Detectplayer == null)
        {
            BookWander();
        }

        else
        {

        }
    }

    void DetectPlayer()
    {
        Detectplayer = Physics2D.OverlapCircle(transform.position, BookSightRange, playerLayerMask);
    }



    void BookWander()
    {
        nowEnemyLocation = new Vector2(transform.position.x, transform.position.y);

        if (isFacingRight == true)
        {
            Invoke("MoveRight", 3.0f);
        }

        if (isFacingRight == false)
        {
            Invoke("MoveLeft", 3.0f);
        }

        #region
        //nowEnemyLocation = new Vector2(transform.position.x, transform.position.y);

        //if(isFacingRight == true && transform.rotation.y ==0)
        //{
        //    EnemyBookAnimator.SetBool("Book_isUturn", false);
        //    Invoke("MoveRight", 3.0f);
        //    Debug.Log(Vector2.Distance(firstEnemyLocation, nowEnemyLocation) >= moveDistance);

        //    if (Vector2.Distance(firstEnemyLocation, nowEnemyLocation) >= moveDistance)
        //    {
        //        MoveStop();
        //        EnemyBookAnimator.SetBool("Book_isUturn", true);

        //        transform.rotation = new Quaternion(0, 0, 0, 0);

        //        isFacingRight = false;

        //    }
        //}

        //if(isFacingRight == false && transform.rotation.y == 180)
        //{
        //    EnemyBookAnimator.SetBool("Book_isUturn", false);
        //    Invoke("MoveLeft", 3.0f);

        //    if (Vector2.Distance(firstEnemyLocation, nowEnemyLocation) >= 0)
        //    {
        //        MoveStop();
        //        EnemyBookAnimator.SetBool("Book_isUturn", true);

        //        transform.rotation = new Quaternion(0, 180, 0, 0);

        //        isFacingRight = true;

        //    }          
        //}
        #endregion
    }


    public override void MoveRight()
    {
        base.MoveRight();

        if (nowEnemyLocation.x - leftEnemyLocation.x >= moveDistance)
        {
            MoveStop();
            rightEnemyLocation = new Vector2(transform.position.x, transform.position.y);

            if (isUturn == true)
            {
                BookAnimatorPlayer.Play(uturnClip);
                isUturn = false;
            }

            if (BookAnimatorPlayer.isEndofFrame)
            {
                BookAnimatorPlayer.Play(idleClip);
            }
            Invoke("Turn", 0.36f);
            isFacingRight = false;
        }
    }

    public override void MoveLeft()
    {
        base.MoveLeft();

        if (rightEnemyLocation.x - nowEnemyLocation.x >= moveDistance)
        {
            MoveStop();
            if (isUturn == false)
            {
                BookAnimatorPlayer.Play(uturnClip);
                isUturn = true;
            }

            if (BookAnimatorPlayer.isEndofFrame)
            {
                BookAnimatorPlayer.Play(idleClip);
            }
            Invoke("Turn", 0.36f);
            isFacingRight = true;
        }
    }

    public void Turn()
    {
        if (isFacingRight == false)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        else if (isFacingRight == true)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

}
