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
    float BookSightRange;

    bool isAttacking = false;
    public GameObject PaperPlaneMuzzle;

    public float moveElapsedTime;
    public float moveTime = 4.0f;
    float moveDistance;
    bool isUTurn = true;
    bool isFacingRight;

    int facingRight;
    Collider2D player;
    public Vector2 firstEnemyLocation;
    public Vector2 nowEnemyLocation;

    void Start()
    {
        enemyHealth = 15;
        EnemyBookAnimator = GetComponent<Animator>();
        BookSightRange = 10;
        moveDistance = 4.0f;
        firstEnemyLocation = new Vector2(transform.position.x, transform.position.y);
        isFacingRight = true;
    }

    void Update()
    {
        DetectPlayer();

        if (player == null)
        {
            BookWander();
        }

        else
        {

        }
    }

    void DetectPlayer()
    {
        player = Physics2D.OverlapCircle(transform.position, BookSightRange, playerLayerMask);
    }

    void BookWander()
    {
        nowEnemyLocation = new Vector2(transform.position.x, transform.position.y);

        if(isFacingRight == true && transform.rotation.y ==0)
        {
            EnemyBookAnimator.SetBool("Book_isUturn", false);
            Invoke("MoveRight", 3.0f);
            Debug.Log(Vector2.Distance(firstEnemyLocation, nowEnemyLocation) >= moveDistance);

            if (Vector2.Distance(firstEnemyLocation, nowEnemyLocation) >= moveDistance)
            {
                MoveStop();
                EnemyBookAnimator.SetBool("Book_isUturn", true);
                
                transform.rotation = new Quaternion(0, 0, 0, 0);

                isFacingRight = false;
                
            }
        }
        
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
       
    }

    void FacingRight()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    void FacingLeft()
    {
        transform.rotation = new Quaternion(0, 180, 0, 0);
    }
}
