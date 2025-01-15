using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Spooksmen_AI : Walker
{
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

    [Header("공격")]
    [SerializeField] float attackRange;
    [SerializeField] float attackCoolTime;
    [SerializeField] GameObject bangMark;

    Animator myAnimator;
    AnimatorPlayer myPlayer;

    BoxCollider2D boxCollider;
    CapsuleCollider2D damageCollider;
    CircleCollider2D circleCollider;

    Collider2D player;

    Transform targetPos;
    // 공격하기 위해 플레이어가 있는 위치 담을 변수
    Transform attackPos;

    // 만들어진 위치
    Vector2 createdPos;
    Vector2 destination;

    GameObject surprised;
    GameObject bang;

    float homeDistance;
    float attackElapsedTime;

    int attackNum;

    bool isHome;
    bool isOverturn;
    bool isAttacking;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myPlayer = GetComponent<AnimatorPlayer>();

        boxCollider = GetComponent<BoxCollider2D>();
        damageCollider = GetComponent<CapsuleCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        boxCollider.enabled = false;
        circleCollider.enabled = false;

        // 놀란 이펙트 생성
        surprised = Instantiate(surprisedMark, surprisedPos);
        surprised.SetActive(false);

        bang = Instantiate(bangMark, surprisedPos);
        bang.SetActive(false);

        createdPos = transform.position;
        homeDistance = 0.5f;

        destination = new Vector2(createdPos.x + wonderRange, 0);

        isHome = true;
        isAttacking = false;
    }

    void Update()
    {
        attackElapsedTime += Time.deltaTime;
        if(isAttacking && attackCoolTime < attackElapsedTime)
        {
            //AttackPlayer();
            //attackElapsedTime = 0;
            attackPos = player.transform;
            AttackPlayer();
        }

        if(isAttacking == false)
        {
            DetectPlayer();
        }
    }

    // 플레이어 감지하는 함수 => 공격하는 것도 만들어야 함
    void DetectPlayer()
    {
        player = Physics2D.OverlapCircle(transform.position, sightRange, playerMask);
        if (player != null)
        {
            if(Physics2D.OverlapCircle(transform.position, attackRange, playerMask) != null)
            {
                isAttacking = true;
            }
            else
            {
                isAttacking = false;
            }

            targetPos = player.transform;
            if (isHome)
            {
                // 플레이어와 내가 같은 방향으로 있으면
                if (transform.rotation.eulerAngles.y == targetPos.rotation.eulerAngles.y)
                {
                    // 플레이어와 반대로 돌아보게 함
                    transform.rotation = Quaternion.Euler(0, Mathf.Abs(targetPos.rotation.eulerAngles.y - 180), 0);
                    isOverturn = true;
                }
                // 플레이어와 내가 같은 방향을 보고 있지 않으면
                else
                {
                    isOverturn = false;
                }

                // 같은 방향으로 있으면
                if (isOverturn)
                {
                    // Sprite Renderer을 flip.X를 해서 플레이어를 바라보지 않고 놀라는 애니메이션 실행시켜야 함
                    myPlayer.Play(surprisedClip, idleClip, true, false);
                }
                // 같은 방향으로 있지 않으면
                else
                {
                    // Sprite Renderer을 flip.X를 하지 않고 놀라는 애니메이션 실행시켜야 함
                    // flip.X를 true로 하면 뒤돌아서 놀라고 원위치로 다시 돌게 됨
                    myPlayer.Play(surprisedClip, idleClip, false, false);
                }

                surprised.SetActive(true);
                isHome = false;
            }
            if(myPlayer.isEndofFrame)
            {
                surprised.SetActive(false);
                // 플레이어 따라다니는 동작 실행
                MovePosition(targetPos.position.x);
            }

        }
        else
        {
            // 내 자리가 아니면 다시 돌아가기
            if(createdPos.x != transform.position.x && isHome == false)
            {
                // 돌아왔으면 자리 범위 돌아다니기
                if(Mathf.Abs(createdPos.x - transform.position.x) < homeDistance)
                {
                    transform.position = new Vector2(createdPos.x, transform.position.y);
                    // 생성된 위치로 돌아가면 돌아다니기 위해 위치 저장
                    destination = new Vector2(transform.position.x + wonderRange, 0);
                    isHome = true;
                }
                // 아직 도착하지 않았으면
                else
                {
                    // 만들어진 장소로 돌아가는 동작 실행
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

    // 0도로 돌아가 있으면 유턴해서 왼쪽으로 움직이는 동작 실행
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

    // 180도로 돌아가 있으면 유턴해서 오른쪽으로 움직이는 동작 실행
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

    // 적이 움직여야 하는 자리까지 이동시키는 함수
    void MovePosition(float targetPosX)
    {
        if(transform.position.x <= targetPosX)
        {
            MoveRight();
        }
        else
        {
            MoveLeft();
        }
    }

    // 플레이어 만나기 전에 돌아다니기
    void WonderSpotRange()
    {
        // 나의 위치와 도착지의 거리 차이가 0보다 크면
        if ((int)Vector2.Distance(transform.position, destination) > 0)
        {
            MovePosition(destination.x);
        }
        else
        {
            // 배회하는 범위가 양수면
            if (wonderRange > 0)
            {
                // 배회 범위를 음수로 수정
                wonderRange = -wonderRange;
            }
            // 배회하는 범위가 음수면
            else
            {
                // 배회 범위를 양수로 수정
                wonderRange = Mathf.Abs(wonderRange);
            }
            // destination 수정
            destination = new Vector2(createdPos.x + wonderRange, 0);
        }
    }

    // 공격
    void AttackPlayer()
    {
        // 공격 패턴
        // 1. (전방) 잽(Attack01) - 어퍼컷(Attack02) - 슬램(Attack03) 순서로 전방에 주먹을 휘두름 -> 피해량 12 - 10 - 11
        // 2. (후방) 몸을 빠르게 회전시키며 앞뒤의 짧은 거리를 공격(Attack04) -> 피해량 10
        // 3. (상향) 어퍼컷을 날린다.(Attack02) -> 피해량 10

        // 공격 범위 안에 들어왔을 때의 플레이어 위치를 포착하는 듯
        // 공격 범위 안에 들어왔을 때 플레이어가 앞에 있으면 전방 공격을 하고
        //                          플레이어가 뒤에 있으면 후방 공격을 하고
        //                          플레이어가 위에 있으면 상향 공격을 함
        // 쿨타임 넣어야 함

        targetPos = player.transform;
        MovePosition(targetPos.position.x);

        if (attackPos.position.y > transform.position.y)
        {
            //UpAttack();
            Debug.Log("상향 공격");
        }
        // 플레이어와 내가 같은 방향인데
        else if (attackPos.rotation.eulerAngles.y == targetPos.rotation.eulerAngles.y)
        {
            //BackAttack();
            Debug.Log("후방 공격");
        }

        else if (attackPos.rotation.eulerAngles.y != targetPos.rotation.eulerAngles.y)
        {
            //FrontAttack();
            Debug.Log("전방 공격");
        }
    }

    void FrontAttack()
    {
        //myAnimator.SetInteger("AttackNum", 1);
        // 피해량 추가 필요

        Debug.Log("전방 공격");
    }

    void BackAttack()
    {
        myAnimator.SetBool("isBackAttack", true);
        // 피해량 추가 필요

        // 박스 Collider enabled 켜기
        boxCollider.enabled = true;

        Debug.Log("후방 공격");
        // 공격 끝나면 박스 Collider enabled 끄기
    }

    void UpAttack()
    {
        myAnimator.SetBool("isUpAttack", true);
        // 피해량 추가 필요

        // 캡슐 Collider Offset, Size 수정 필요
        damageCollider.offset = new Vector2(-0.11f, 0.34f);
        damageCollider.size = new Vector2(3.34f, 4.4f);

        Debug.Log("상향 공격");
        // 공격 끝나면 원래 캡슐 Collider Offset, Size로 돌아오기
    }
}
