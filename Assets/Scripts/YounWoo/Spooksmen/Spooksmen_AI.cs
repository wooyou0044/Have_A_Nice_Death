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

    [Header("플레이어 감지")]
    // 감지하는 범위
    [SerializeField] float sightRange;
    // 플레이어 감지했을 때 켜둘 느낌표 오브젝트
    [SerializeField] GameObject surprisedMark;
    [SerializeField] Transform surprisedPos;
    [SerializeField] LayerMask playerMask;

    Animator myAnimator;

    Transform targetPos;
    // 만들어진 위치
    Transform createdPos;

    GameObject surprised;
    // 임시
    int count = 0;

    float uTurn;

    bool isRight;
    bool isMove;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        // 놀란 이펙트 생성
        surprised = Instantiate(surprisedMark, surprisedPos);
        surprised.SetActive(false);
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
            // 플레이어가 발견될 때 한번만 실행 -> 따라다니다가 내 자리로 돌아가고 나서 돌아다니다가 다시 플레이어 발견했을 때 실행 (FollowPlayer)
            // 임시로
            count++;
            if (count==1)
            {
                myAnimator.SetTrigger("Detect");
                surprised.SetActive(true);
            }
            else
            {
                surprised.SetActive(false);
            }

            targetPos = player.transform;
            FollowPlayer();
        }
        else
        {
            // 내 자리가 아니면 다시 돌아가고 내 자리면 돌아다니기(ComeBackCreatedPosition, WonderSpotRange)
            //ComeBackCreatedPosition();
            //Debug.Log("createdPos : " + createdPos.position);
            surprised.SetActive(false);
        }
    }

    // 플레이어가 감지되면 따라다니기
    void FollowPlayer()
    {
        if(transform.position.x <= targetPos.position.x)
        {
            // 만약에 지금 rotation y 좌표가 180이면 돌기
            if (transform.rotation.eulerAngles.y >= 180)
            {
                // 돌때는 MoveStop();
                MoveStop();
                isRight = true;
                myAnimator.SetBool("isUturn", true);
            }
            else
            {
                //transform.rotation = Quaternion.Euler(0, 0, 0);
                // 오른쪽으로 이동
                MoveRight();
            }
        }
        else
        {
            // 만약에 지금 rotation y 좌표가 0이면 돌기
            if(transform.rotation.eulerAngles.y <= 0)
            {
                // 돌때는 MoveStop();
                MoveStop();
                isRight = false;
                myAnimator.SetBool("isUturn", true);
            }

            else
            {
                //transform.rotation = Quaternion.Euler(0, 180, 0);
                // 왼쪽으로 이동
                MoveLeft();
            }
        }
    }

    public void UTurn()
    {
        if (isRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        myAnimator.SetBool("isUturn", false);
        //isMove = true;
    }

    // 플레이어가 일정 범위를 벗어나면 내 자리로 돌아오기
    void ComeBackCreatedPosition()
    {
        //if (transform.position.x != createdPos.position.x)
        //{
        //    // 어딜 돌아볼건지는 일단 나중에 설정
        //    myRigid.velocity = new Vector2(-moveSpeed, 0);
        //}

        // 제자리로 돌아오면 isSurprised 켜두기
    }

    // 플레이어 만나기 전에 돌아다니기
    void WonderSpotRange()
    {

    }

    // 플레이어가 왼쪽 오른쪽 이동할 때 Uturn하기

}
