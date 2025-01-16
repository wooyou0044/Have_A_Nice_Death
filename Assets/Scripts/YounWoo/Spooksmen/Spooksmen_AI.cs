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
    [SerializeField] Skill skill2;
    [SerializeField] Skill skill3;

    Animator myAnimator;
    AnimatorPlayer myPlayer;

    BoxCollider2D boxCollider;
    CapsuleCollider2D damageCollider;
    CircleCollider2D circleCollider;

    Collider2D player;
    Collider2D attackPlayer;

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
    bool isAttacking;
    bool isDetect;
    bool isFrontAttack;

    enum AttackState
    {
        Default,
        FrontAttack_Jab,
        FrontAttack_UpperCut,
        FrontAttack_Slam,
        BackAttack,
        UpAttack,
    }
    AttackState state;

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
        isDetect = false;
        attackElapsedTime = attackCoolTime;
        state = AttackState.Default;
    }

    void Update()
    {
        attackPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
        if (attackPlayer != null)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
            DetectPlayer();
        }
        if(isAttacking)
        {
            bang.SetActive(true);
            MovePosition(attackPlayer.transform.position.x);
            attackElapsedTime += Time.deltaTime;
            // 0.5초 지나면 느낌표 끄기
            if(0.5f <= attackElapsedTime)
            {
                bang.SetActive(false);
            }
        }
        if (isAttacking && attackCoolTime <= attackElapsedTime)
        {
            AttackPlayer();
            attackElapsedTime = 0;
            isAttacking = false;
        }
    }

    // 플레이어 감지하는 함수 => 공격하는 것도 만들어야 함
    void DetectPlayer()
    {
        player = Physics2D.OverlapCircle(transform.position, sightRange, playerMask);
        if (player != null)
        {
            isDetect = true;
            targetPos = player.transform;
            if (isHome)
            {
                // 공격 타입 결정
                isFrontAttack = AttackType(targetPos.rotation.eulerAngles.y, transform.rotation.eulerAngles.y);

                myPlayer.Play(surprisedClip, idleClip, false, false);
                surprised.SetActive(true);
                isHome = false;
            }
            if(myPlayer.isEndofFrame)
            {
                surprised.SetActive(false);
                attackPos = targetPos;
                // 플레이어 따라다니는 동작 실행
                MovePosition(targetPos.position.x);
            }
        }
        else
        {
            isDetect = false;
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

    bool AttackType(float targetRotation, float myRotation)
    {
        if (targetRotation == myRotation)
        {
            return false;
        }
        else
        {
            return true;
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

        if(isDetect== false)
        {
            attackPos = attackPlayer.transform;

            isFrontAttack = AttackType(attackPos.rotation.eulerAngles.y, transform.rotation.eulerAngles.y);
        }

        if (attackPos.position.y > transform.position.y)
        {
            UpAttack();
            Debug.Log("상향 공격");
        }
        else if(isFrontAttack)
        {
            FrontAttack();
            Debug.Log("전방 공격");
        }
        else
        {
            BackAttack();
            Debug.Log("후방 공격");
        }
    }

    void FrontAttack()
    {
        //myAnimator.SetInteger("AttackNum", 1);
        // 피해량 추가 필요
        
    }

    void BackAttack()
    {
        myAnimator.SetBool("isBackAttack", true);
        // 피해량 추가 필요

        boxCollider.offset = new Vector2(0, 0.2581731f);
        boxCollider.size = new Vector2(5.85f, 1.372605f);

        // 박스 Collider enabled 켜기
        boxCollider.enabled = true;

        state = AttackState.BackAttack;
        // 공격 끝나면 박스 Collider enabled 끄기
    }

    void UpAttack()
    {
        myAnimator.SetBool("isUpAttack", true);
        // 피해량 추가 필요

        // 캡슐 Collider Offset, Size 수정 필요
        damageCollider.offset = new Vector2(-0.11f, 0.09f);
        damageCollider.size = new Vector2(3.34f, 4.1f);

        state = AttackState.UpAttack;
        // 공격 끝나면 원래 캡슐 Collider Offset, Size로 돌아오기
    }

    public void ResetAttack()
    {
        switch(state)
        {
            case AttackState.FrontAttack_UpperCut:
                break;
            case AttackState.UpAttack:
                damageCollider.offset = new Vector2(0.0899694f, 0);
                damageCollider.size = new Vector2(1.239239f, 2.87f);
                myAnimator.SetBool("isUpAttack", false);
                state = AttackState.Default;
                break;
            case AttackState.BackAttack:
                boxCollider.enabled = false;
                myAnimator.SetBool("isBackAttack", false);
                state = AttackState.Default;
                break;
        }
    }
}
