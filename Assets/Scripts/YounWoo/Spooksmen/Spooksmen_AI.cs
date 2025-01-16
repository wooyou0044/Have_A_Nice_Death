using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Spooksmen_AI : Walker, IHittable
{
    LayerMask playerLayer;

    [SerializeField] AnimationClip uTurnClip;
    [SerializeField] AnimationClip idleClip;
    [SerializeField] AnimationClip surprisedClip;
    [SerializeField] AnimationClip hitClip;
    [SerializeField] AnimationClip stunClip;

    [Header("나의 정보")]
    [SerializeField] float hp = 200.0f;

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
    [SerializeField] GameObject attackMark;
    [SerializeField] Skill[] frontAttack;
    [SerializeField] Skill backAttack;
    [SerializeField] Skill upAttack;

    [Header("데미지")]
    [SerializeField] GameObject stunMark;
    [SerializeField] Transform stunPos;

    Animator myAnimator;
    AnimatorPlayer myPlayer;

    Collider2D player;
    Collider2D attackPlayer;
    Collider2D damageCollider;

    Transform targetPos;
    // 공격하기 위해 플레이어가 있는 위치 담을 변수
    Transform attackPos;
    // attack할 때 나올 Effect 나올 위치 담을 변수
    Transform attackMarkPos;

    // 만들어진 위치
    Vector2 createdPos;
    Vector2 destination;

    GameObject surprised;
    GameObject bang;
    GameObject stun;
    GameObject attack;

    float homeDistance;
    float attackElapsedTime;

    int frontAttackNum;

    bool isHome;
    bool isAttacking;
    bool isDetect;
    bool isFrontAttack;

    enum AttackState
    {
        Default,
        FrontAttack,
        BackAttack,
        UpAttack,
    }
    AttackState state;

    public bool isAlive
    {
        get
        {
            return (hp > 0) ? true : false;
        }
    }

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myPlayer = GetComponent<AnimatorPlayer>();
        damageCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // 놀란 이펙트 생성
        surprised = Instantiate(surprisedMark, surprisedPos);
        surprised.SetActive(false);

        bang = Instantiate(bangMark, surprisedPos);
        bang.SetActive(false);

        stun = Instantiate(stunMark, stunPos);
        stun.SetActive(false);

        attackMarkPos = transform.GetChild(2).transform;
        attack = Instantiate(attackMark, attackMarkPos);
        attack.SetActive(false);

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
            // 전방 공격할 때 플레이어가 범위 안에 없으면 리셋
            frontAttackNum = 0;
            myAnimator.SetInteger("AttackNum", frontAttackNum);

            bangMark.SetActive(false);
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

    bool AttackType(float targetRot, float myRot)
    {
        // 나와 플레이어가 같은 방향을 바라보고 있는 경우
        if (targetRot == myRot)
        {
            return false;
        }
        // 나와 플레이어가 마주보고 있는 경우
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
            Debug.Log("상향 공격");
            UpAttack();
        }
        else if(isFrontAttack)
        {
            Debug.Log("전방 공격");
            FrontAttack();
        }
        else
        {
            Debug.Log("후방 공격");
            BackAttack();
        }

        isDetect = false;
    }

    void FrontAttack()
    {
        //myAnimator.SetInteger("AttackNum", 1);
        // 피해량 추가 필요
        frontAttackNum++;
        myAnimator.SetInteger("AttackNum", frontAttackNum);
        attack.SetActive(true);
        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            yield return new WaitForSeconds(0.5f);
            frontAttack[frontAttackNum - 1].Use(transform, null, new string[] { "Player" }, GameManager.ShowEffect, GameManager.Use, GameManager.GetProjectile);
        }
        state = AttackState.FrontAttack;
    }

    void BackAttack()
    {
        myAnimator.SetBool("isBackAttack", true);

        attack.SetActive(true);

        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            yield return new WaitForSeconds(0.5f);
            backAttack.Use(transform, null, new string[] {"Player"}, GameManager.ShowEffect, GameManager.Use, GameManager.GetProjectile);
        }
        state = AttackState.BackAttack;
    }

    void UpAttack()
    {
        myAnimator.SetBool("isUpAttack", true);
        // 피해량 추가 필요

        attack.SetActive(true);

        StartCoroutine(DoPlay());
        IEnumerator DoPlay()
        {
            yield return new WaitForSeconds(0.5f);
            upAttack.Use(transform, null, new string[] { "Player" }, GameManager.ShowEffect, GameManager.Use, GameManager.GetProjectile);
        }

        state = AttackState.UpAttack;
    }

    public void ResetAttack()
    {
        switch(state)
        {
            // 밑에 꺼지는 것 작동하기 위해서 전방 공격 뒤에 붙여야 함
            case AttackState.FrontAttack:
                if(frontAttackNum >= 3)
                {
                    frontAttackNum = 0;
                    myAnimator.SetInteger("AttackNum", frontAttackNum);
                    state = AttackState.Default;
                }
                break;
            case AttackState.UpAttack:
                myAnimator.SetBool("isUpAttack", false);
                state = AttackState.Default;
                break;
            case AttackState.BackAttack:
                myAnimator.SetBool("isBackAttack", false);
                state = AttackState.Default;
                break;
        }
        bang.SetActive(false);
        attack.SetActive(false);
    }

    public void Hit(Strike strike)
    {
        // 맞는 애니메이션 쓰고
        hp += strike.result;

        if(hp > 0)
        {
            myPlayer.Play(hitClip, idleClip, false);
            Debug.Log("myHp : " + hp);

            // 특정 공격 받을 때 스턴 걸림
        }

        else
        {
            // 죽는 모션
        }

        //if(myPlayer.isEndofFrame)
        //{
            
        //}
    }

    public Collider2D GetCollider2D()
    {
        return damageCollider;
    }
}
