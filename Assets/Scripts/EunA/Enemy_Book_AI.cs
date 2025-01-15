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

    #region 애니메이션 클립
    public AnimationClip idleClip;
    public AnimationClip uturnClip;
    public AnimationClip findClip;
    public AnimationClip hitClip;
    public AnimationClip attackClip;
    public AnimationClip dieClip;
    #endregion

    float BookSightRange;
    float BookAttackRange;
    bool isFacingRight = true;
    bool isUturn = true;
    bool isFind = true;
    bool isAttack = false;
    int facingRight;

    float BookFindCooltime;
    float BookFindElapsedtime;


    Rigidbody2D enemyRigid;
    Collider2D detectplayerErea;
    Collider2D attackplayerErea;
    Vector2 leftEnemyLocation;
    Vector2 nowEnemyLocation;
    Vector2 rightEnemyLocation;
    Transform target;

    [SerializeField]
    AnimatorPlayer BookAnimatorPlayer;
    [SerializeField]
    float moveDistance;
    [SerializeField]
    LayerMask playerLayerMask;


    void Start()
    {
        enemyHealth = 15;
        EnemyBookAnimator = GetComponent<Animator>();
        BookSightRange = 5;
        BookAttackRange = 4;
        moveDistance = 4.0f;
        leftEnemyLocation = new Vector2(transform.position.x, transform.position.y);
        BookAnimatorPlayer = GetComponent<AnimatorPlayer>();
        BookFindCooltime = 4.0f;
        BookFindElapsedtime = 4.0f;
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        detectplayerErea = Physics2D.OverlapCircle(transform.position, BookSightRange, playerLayerMask);

        if (detectplayerErea != null)
        {
            FindPlayer();
        }

        else if(BookAnimatorPlayer.isEndofFrame || detectplayerErea == null)
        {
            BookWander();
            isFind = true;
            BookFindElapsedtime = 4.0f;
        }
    }

    void FindPlayer()
    {
        MoveStop();
        BookFindElapsedtime += Time.deltaTime;

        if (isFind == true)
        {
            Debug.Log("놀람");
            BookAnimatorPlayer.Play(findClip, idleClip);
            isFind = false;
        }

        if (BookAnimatorPlayer.isEndofFrame && BookFindElapsedtime >= BookFindCooltime) 
        {
            Debug.Log("공격");
            BookAnimatorPlayer.Play(attackClip, idleClip);

            //if(detectplayerErea == null)
            //{
            //    BookAnimatorPlayer.Play(idleClip);
            //}

            BookFindElapsedtime = 0;
        }

        else if(BookAnimatorPlayer.isEndofFrame)
        {
            BookAnimatorPlayer.Play(idleClip);
        }
        
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

            if (isUturn == true && gameObject.transform.eulerAngles == new Vector3(0, 0))
            {
                BookAnimatorPlayer.Play(uturnClip);
                isUturn = false;
            }

            if (BookAnimatorPlayer.isEndofFrame)
            {
                BookAnimatorPlayer.Play(idleClip);
            }
            Invoke("Turn", 0.36f);

            if (gameObject.transform.rotation == Quaternion.Euler(0, 180, 0))
            {
                isUturn = true;
            }
            isFacingRight = false;
        }
    }

    public override void MoveLeft()
    {
        base.MoveLeft();

        if (rightEnemyLocation.x - nowEnemyLocation.x >= moveDistance)
        {
            MoveStop();
            if (isUturn == true && gameObject.transform.eulerAngles == new Vector3(0, 180))
            {
                BookAnimatorPlayer.Play(uturnClip);
                isUturn = false;
            }

            if (BookAnimatorPlayer.isEndofFrame)
            {
                BookAnimatorPlayer.Play(idleClip);
            }
            Invoke("Turn", 0.36f);

            if (gameObject.transform.rotation == Quaternion.Euler(0, 0, 0))
            {
                isUturn = true;
            }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, BookSightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, BookAttackRange);
    }

}
