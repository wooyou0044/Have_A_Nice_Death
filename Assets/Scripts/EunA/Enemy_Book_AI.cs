using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy_Book_AI : Walker, IHittable
{
    int MaxEnemyHealth;
    int NowEnemyHealth;
    Animator EnemyBookAnimator;

    #region 애니메이션 클립
    public AnimationClip idleClip;
    public AnimationClip uturnClip;
    public AnimationClip findClip;
    public AnimationClip hitClip;
    public AnimationClip attackClip;
    public AnimationClip dieClip;
    #endregion

    float detectTime;
    float BookSightRange;
    bool isFacingRight = true;
    bool isUturn = true;
    bool isFind = true;
    bool isAttack = false;
    bool AttackAnimationIsOver = true;
    int facingRight;

    float BookFindCooltime;
    float BookFindElapsedtime;

    Rigidbody2D enemyRigid;
    Collider2D enemyCollider;
    Collider2D detectplayerErea;
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

    bool IHittable.isAlive => MaxEnemyHealth > 0;

    string IHittable.tag { get; set; }

    Transform IHittable.transform => transform;

    void Start()
    {
        MaxEnemyHealth = 15;
        NowEnemyHealth = 15;
        EnemyBookAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
        BookSightRange = 5;
        moveDistance = 6.0f;
        leftEnemyLocation = new Vector2(transform.position.x, transform.position.y);
        BookAnimatorPlayer = GetComponent<AnimatorPlayer>();
        BookFindCooltime = 4.0f;
        BookFindElapsedtime = 4.0f;
    }

    void Update()
    {
        if (BookAnimatorPlayer.IsPlaying(findClip) != true && BookAnimatorPlayer.IsPlaying(attackClip) != true)
        {
            DetectPlayer();
        }
    }

    void DetectPlayer()
    {
        detectplayerErea = Physics2D.OverlapCircle(transform.position, BookSightRange, playerLayerMask);

        if (detectplayerErea != null)
        {
            FindPlayer();
        }

        else if (detectplayerErea == null)
        {
            Debug.Log("방황 시작");
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
            BookFindElapsedtime = 0;
        }

        else if (BookAnimatorPlayer.isEndofFrame)
        {
            BookAnimatorPlayer.Play(idleClip);
        }

    }

    void BookWander()
    {
        nowEnemyLocation = new Vector2(transform.position.x, transform.position.y);

        if (isFacingRight == true)
        {
            MoveRight();
        }

        if (isFacingRight == false)
        {
            MoveLeft();
        }
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
    }

    void IHittable.Hit(Strike strike)
    {
        if (NowEnemyHealth < MaxEnemyHealth / 2)
        {
            MoveStop();
            BookAnimatorPlayer.Play(hitClip);
        }
    }

    Collider2D IHittable.GetCollider2D()
    {
        return enemyCollider;
    }
}
