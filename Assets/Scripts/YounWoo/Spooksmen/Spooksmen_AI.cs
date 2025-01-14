using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Spooksmen_AI : Walker
{
    // 공격 패턴
    // 1. (전방) 잽(Attack01) - 어퍼컷(Attack02) - 슬램(Attack03) 순서로 전방에 주먹을 휘두름 -> 피해량 12 - 10 - 11
    // 2. (후방) 몸을 빠르게 회전시키며 앞뒤의 짧은 거리를 공격(Attack04) -> 피해량 10
    // 3. (상향) 어퍼컷을 날린다.(Attack02) -> 피해량 10

    LayerMask playerLayer;

    [SerializeField] AnimationClip uTurnClip;
    [SerializeField] AnimationClip idleClip;
    [SerializeField] AnimationClip surprisedClip;

    [Header("플레이어 감지")]
    // 감지하는 범위
    [SerializeField] float sightRange;
    // 플레이어 감지했을 때 켜둘 느낌표 오브젝트
    [SerializeField] GameObject surprisedMark;
    [SerializeField] Transform surprisedPos;
    [SerializeField] LayerMask playerMask;

    [Header("배회")]
    [SerializeField] float wonderRange;

    Animator myAnimator;
    AnimatorPlayer myPlayer;

    Transform targetPos;
    // 만들어진 위치
    Vector2 createdPos;
    Vector2 destination;

    GameObject surprised;

    float homeDistance;

    bool isHome;
    bool isOverturn;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myPlayer = GetComponent<AnimatorPlayer>();
    }

    void Start()
    {
        // 놀란 이펙트 생성
        surprised = Instantiate(surprisedMark, surprisedPos);
        surprised.SetActive(false);

        createdPos = transform.position;
        homeDistance = 0.5f;
        isHome = true;
    }

    void Update()
    {
        DetectPlayer();
    }

    // 플레이어 감지하는 함수 => 공격하는 것도 만들어야 함
    void DetectPlayer()
    {
        Collider2D player;
        player = Physics2D.OverlapCircle(transform.position, sightRange, playerMask);
        if (player != null)
        {
            targetPos = player.transform;
            if (isHome)
            {
                if(transform.rotation.eulerAngles.y == targetPos.rotation.eulerAngles.y)
                {
                    transform.rotation = Quaternion.Euler(0, Mathf.Abs(targetPos.rotation.eulerAngles.y - 180), 0);
                    isOverturn = true;
                }
                else
                {
                    isOverturn = false;
                }

                if(isOverturn)
                {
                    myPlayer.Play(surprisedClip, idleClip, true, false);
                }
                else
                {
                    myPlayer.Play(surprisedClip, idleClip, false, false);
                }

                surprised.SetActive(true);
                isHome = false;
            }
            if(myPlayer.isEndofFrame)
            {
                surprised.SetActive(false);
                //FollowPlayer();
                MovePosition(targetPos.position.x);
            }

        }
        else
        {
            // 내 자리가 아니면 다시 돌아가기
            if(createdPos.x != transform.position.x)
            {
                // 돌아왔으면 자리 범위 돌아다니기
                if(Mathf.Abs(createdPos.x - transform.position.x) < homeDistance)
                {
                    transform.position = new Vector2(createdPos.x, transform.position.y);
                    // 생성된 위치로 돌아가면 돌아다니기 위해 위치 저장
                    destination = new Vector2(transform.position.x + wonderRange, transform.position.y);
                    isHome = true;
                }
                else
                {
                    //ComeBackCreatedPosition();
                    MovePosition(createdPos.x);
                }
            }

            // 내 자리면 돌아다니기(WonderSpotRange)
            if(isHome)
            {
                WonderSpotRange();
            }
            surprised.SetActive(false);
        }
    }

    // 플레이어가 감지되면 따라다니기
    //void FollowPlayer()
    //{
    //    if(transform.position.x <= targetPos.position.x)
    //    {
    //        MoveRight();
    //    }
    //    else
    //    {
    //        MoveLeft();
    //    }
    //}

    public override void MoveLeft()
    {
        if (transform.rotation.eulerAngles.y <= 0)
        {
            MoveStop();
            myPlayer.Play(uTurnClip, idleClip, true);
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        if(myPlayer.isEndofFrame)
        {
            base.MoveLeft();
        }
    }

    public override void MoveRight()
    {
        if (transform.rotation.eulerAngles.y >= 180)
        {
            MoveStop();
            myPlayer.Play(uTurnClip, idleClip, true);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (myPlayer.isEndofFrame)
        {
            base.MoveRight();
        }
    }

    // 플레이어가 일정 범위를 벗어나면 내 자리로 돌아오기
    //void ComeBackCreatedPosition()
    //{
    //    if (transform.position.x < createdPos.x)
    //    {
    //        MoveRight();
    //    }

    //    else if (transform.position.x > createdPos.x)
    //    {
    //        MoveLeft();
    //    }
    //}

    void MovePosition(float posX)
    {
        if(transform.position.x <= posX)
        {
            MoveRight();
        }
        else
        {
            MoveLeft();
        }
    }

    public void SurprisedNotActive()
    {
        surprised.SetActive(false);
    }

    // 플레이어 만나기 전에 돌아다니기
    void WonderSpotRange()
    {
        Debug.Log(Vector2.Distance(transform.position, destination));
        if(Vector2.Distance(transform.position, destination) > 0)
        {
            if(transform.position.x <= destination.x)
            {
                base.MoveRight();
            }
            else
            {
                base.MoveLeft();
            }
        }
    }

    // 공격
    void Attack()
    {

    }

}
