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
    public int MaxEnemyHealth;
    public int NowEnemyHealth;
    public ParticleSystem StunEffect;
    Animator EnemyBookAnimator;

    public GameObject FindEffect;
    public GameObject AttackEffect;

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
    float moveCooltime;
    float moveElapsedtime;

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

    public bool isAlive
    {
        get
        {
            return MaxEnemyHealth > 0;
        }
    }

    Transform IHittable.transform => transform;

    void Start()
    {
        MaxEnemyHealth = 15;
        NowEnemyHealth = 15;
        BookSightRange = 5;
        moveCooltime = 3.0f;
        moveElapsedtime = 0;
        BookFindCooltime = 4.0f;
        BookFindElapsedtime = 4.0f;
        moveDistance = 8.0f;
        EnemyBookAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();       
        leftEnemyLocation = new Vector2(transform.position.x, transform.position.y);
        BookAnimatorPlayer = GetComponent<AnimatorPlayer>();
        StunEffect = GetComponent<ParticleSystem>();
        //FindEffect = GameObject.Find("Find");
        //AttackEffect = GameObject.Find("Attack_Effect");
    }

    void Update()
    {
        if (BookAnimatorPlayer.IsPlaying(findClip) != true && BookAnimatorPlayer.IsPlaying(attackClip) != true)
        {
            DetectPlayer();
        }

        if (NowEnemyHealth <= 0)
        {
            MoveStop();
            Die();
        }
        Debug.Log(isUturn);
        //Debug.Log(moveElapsedtime);
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
            FindEffect.SetActive(true);
            BookAnimatorPlayer.Play(findClip, idleClip);
            isFind = false;
        }

        if (BookAnimatorPlayer.isEndofFrame && BookFindElapsedtime >= BookFindCooltime)
        {
            Debug.Log("공격");
            AttackEffect.SetActive(true);
            BookAnimatorPlayer.Play(attackClip, idleClip);

            BookFindElapsedtime = 0;
        }

        else if (BookAnimatorPlayer.isEndofFrame)
        {
            BookAnimatorPlayer.Play(idleClip);
            FindEffect.SetActive(false);
            AttackEffect.SetActive(false);
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
            moveElapsedtime += Time.deltaTime;

            if (moveElapsedtime >= moveCooltime)
            {
                rightEnemyLocation = new Vector2(transform.position.x, transform.position.y);

                if (isUturn == true)
                {
                    BookAnimatorPlayer.Play(uturnClip, idleClip);
                    isUturn = false;
                }

                if(isUturn == false && BookAnimatorPlayer.IsPlaying(uturnClip) != true)
                {
                    Invoke("Turn", 0.36f);
                    isUturn = true;
                    moveElapsedtime = 0;
                    isFacingRight = false;
                }
                
            }
        }        
    }

    public override void MoveLeft()
    {      
        base.MoveLeft();

        if (rightEnemyLocation.x - nowEnemyLocation.x >= moveDistance )
        {
            MoveStop();
            moveElapsedtime += Time.deltaTime;

            if (moveElapsedtime >= moveCooltime)
            {              
                if (isUturn == true)
                {
                    BookAnimatorPlayer.Play(uturnClip, idleClip);
                    isUturn = false;
                }

                if (isUturn == false && BookAnimatorPlayer.IsPlaying(uturnClip) != true)
                {
                    Invoke("Turn", 0.36f);
                    isUturn = true;
                    moveElapsedtime = 0;
                    isFacingRight = true;
                }


            }
        }           
    }

    public void Turn()
    {
        if (isFacingRight == false)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        else if (isFacingRight == true)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, BookSightRange);
    }

    public void Hit(Strike strike)
    {
        NowEnemyHealth += strike.result;

        if (NowEnemyHealth < MaxEnemyHealth / 2)
        {
            MoveStop();
            BookAnimatorPlayer.Play(hitClip);
        }
    }

    public Collider2D GetCollider2D()
    {
        return enemyCollider;
    }

    void Die()
    {
        BookAnimatorPlayer.Play(dieClip);
    }
}
