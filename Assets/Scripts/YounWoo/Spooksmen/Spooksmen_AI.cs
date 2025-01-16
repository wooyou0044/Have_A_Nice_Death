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

    [Header("���� ����")]
    [SerializeField] float hp = 200.0f;

    [Header("�÷��̾� ����")]
    // �����ϴ� ����
    [SerializeField] float sightRange;
    // �÷��̾� �������� �� �ѵ� ����ǥ ������Ʈ
    [SerializeField] GameObject surprisedMark;
    [SerializeField] Transform surprisedPos;
    [SerializeField] LayerMask playerMask;

    [Header("��ȸ")]
    [SerializeField] float wonderRange;

    [Header("����")]
    [SerializeField] float attackRange;
    [SerializeField] float attackCoolTime;
    [SerializeField] GameObject bangMark;
    [SerializeField] Skill[] frontAttack;
    [SerializeField] Skill backAttack;
    [SerializeField] Skill upAttack;

    [Header("������")]
    [SerializeField] GameObject stunMark;
    [SerializeField] Transform stunPos;

    Animator myAnimator;
    AnimatorPlayer myPlayer;

    Collider2D player;
    Collider2D attackPlayer;
    Collider2D damageCollider;

    Transform targetPos;
    // �����ϱ� ���� �÷��̾ �ִ� ��ġ ���� ����
    Transform attackPos;

    // ������� ��ġ
    Vector2 createdPos;
    Vector2 destination;

    GameObject surprised;
    GameObject bang;
    GameObject stun;

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
        // ��� ����Ʈ ����
        surprised = Instantiate(surprisedMark, surprisedPos);
        surprised.SetActive(false);

        bang = Instantiate(bangMark, surprisedPos);
        bang.SetActive(false);

        stun = Instantiate(stunMark, stunPos);
        stun.SetActive(false);

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
            bangMark.SetActive(false);
            isAttacking = false;
            DetectPlayer();
        }
        if(isAttacking)
        {
            bang.SetActive(true);
            MovePosition(attackPlayer.transform.position.x);
            attackElapsedTime += Time.deltaTime;
            // 0.5�� ������ ����ǥ ����
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

    // �÷��̾� �����ϴ� �Լ� => �����ϴ� �͵� ������ ��
    void DetectPlayer()
    {
        player = Physics2D.OverlapCircle(transform.position, sightRange, playerMask);
        if (player != null)
        {
            isDetect = true;
            targetPos = player.transform;
            if (isHome)
            {
                // ���� Ÿ�� ����
                isFrontAttack = AttackType(targetPos.rotation.eulerAngles.y, transform.rotation.eulerAngles.y);

                myPlayer.Play(surprisedClip, idleClip, false, false);
                surprised.SetActive(true);
                isHome = false;
            }
            if(myPlayer.isEndofFrame)
            {
                surprised.SetActive(false);
                attackPos = targetPos;
                // �÷��̾� ����ٴϴ� ���� ����
                MovePosition(targetPos.position.x);
            }
        }
        else
        {
            isDetect = false;
            // �� �ڸ��� �ƴϸ� �ٽ� ���ư���
            if(createdPos.x != transform.position.x && isHome == false)
            {
                // ���ƿ����� �ڸ� ���� ���ƴٴϱ�
                if(Mathf.Abs(createdPos.x - transform.position.x) < homeDistance)
                {
                    transform.position = new Vector2(createdPos.x, transform.position.y);
                    // ������ ��ġ�� ���ư��� ���ƴٴϱ� ���� ��ġ ����
                    destination = new Vector2(transform.position.x + wonderRange, 0);
                    isHome = true;
                }
                // ���� �������� �ʾ�����
                else
                {
                    // ������� ��ҷ� ���ư��� ���� ����
                    MovePosition(createdPos.x);
                }
            }

            // �� �ڸ��� ���ƴٴϱ�(WonderSpotRange)
            if(isHome)
            {
                WonderSpotRange();
            }
            surprised.SetActive(false);
        }
    }

    // 0���� ���ư� ������ �����ؼ� �������� �����̴� ���� ����
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

    // 180���� ���ư� ������ �����ؼ� ���������� �����̴� ���� ����
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

    // ���� �������� �ϴ� �ڸ����� �̵���Ű�� �Լ�
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

    // �÷��̾� ������ ���� ���ƴٴϱ�
    void WonderSpotRange()
    {
        // ���� ��ġ�� �������� �Ÿ� ���̰� 0���� ũ��
        if ((int)Vector2.Distance(transform.position, destination) > 0)
        {
            MovePosition(destination.x);
        }
        else
        {
            // ��ȸ�ϴ� ������ �����
            if (wonderRange > 0)
            {
                // ��ȸ ������ ������ ����
                wonderRange = -wonderRange;
            }
            // ��ȸ�ϴ� ������ ������
            else
            {
                // ��ȸ ������ ����� ����
                wonderRange = Mathf.Abs(wonderRange);
            }
            // destination ����
            destination = new Vector2(createdPos.x + wonderRange, 0);
        }
    }

    bool AttackType(float targetRot, float myRot)
    {
        // ���� �÷��̾ ���� ������ �ٶ󺸰� �ִ� ���
        if (targetRot == myRot)
        {
            return false;
        }
        // ���� �÷��̾ �ٸ� ������ �ٶ󺸰� �ִ� ���
        else
        {
            return true;
        }
    }

    // ����
    void AttackPlayer()
    {
        // ���� ����
        // 1. (����) ��(Attack01) - ������(Attack02) - ����(Attack03) ������ ���濡 �ָ��� �ֵθ� -> ���ط� 12 - 10 - 11
        // 2. (�Ĺ�) ���� ������ ȸ����Ű�� �յ��� ª�� �Ÿ��� ����(Attack04) -> ���ط� 10
        // 3. (����) �������� ������.(Attack02) -> ���ط� 10

        // ���� ���� �ȿ� ������ ���� �÷��̾� ��ġ�� �����ϴ� ��
        // ���� ���� �ȿ� ������ �� �÷��̾ �տ� ������ ���� ������ �ϰ�
        //                          �÷��̾ �ڿ� ������ �Ĺ� ������ �ϰ�
        //                          �÷��̾ ���� ������ ���� ������ ��

        if(isDetect== false)
        {
            attackPos = attackPlayer.transform;

            isFrontAttack = AttackType(attackPos.rotation.eulerAngles.y, transform.rotation.eulerAngles.y);
        }

        if (attackPos.position.y > transform.position.y)
        {
            UpAttack();
            Debug.Log("���� ����");
        }
        else if(isFrontAttack)
        {
            FrontAttack();
            Debug.Log("���� ����");
        }
        else
        {
            BackAttack();
            Debug.Log("�Ĺ� ����");
        }

        isDetect = false;
    }

    void FrontAttack()
    {
        //myAnimator.SetInteger("AttackNum", 1);
        // ���ط� �߰� �ʿ�
        

    }

    void BackAttack()
    {
        myAnimator.SetBool("isBackAttack", true);

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
        // ���ط� �߰� �ʿ�

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
            case AttackState.FrontAttack_UpperCut:
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
    }

    public void Hit(Strike strike)
    {
        // �´� �ִϸ��̼� ����
        hp += strike.result;

        if(hp > 0)
        {
            myPlayer.Play(hitClip, idleClip, false);
            Debug.Log("myHp : " + hp);
        }
        else
        {
            // �״� ���
        }
    }

    public Collider2D GetCollider2D()
    {
        return damageCollider;
    }
}
