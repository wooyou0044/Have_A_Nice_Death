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

    Animator myAnimator;
    AnimatorPlayer myPlayer;

    BoxCollider2D boxCollider;
    CapsuleCollider2D damageCollider;
    CircleCollider2D circleCollider;

    Collider2D player;

    Transform targetPos;
    // �����ϱ� ���� �÷��̾ �ִ� ��ġ ���� ����
    Transform attackPos;

    // ������� ��ġ
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

        // ��� ����Ʈ ����
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

    // �÷��̾� �����ϴ� �Լ� => �����ϴ� �͵� ������ ��
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
                // �÷��̾�� ���� ���� �������� ������
                if (transform.rotation.eulerAngles.y == targetPos.rotation.eulerAngles.y)
                {
                    // �÷��̾�� �ݴ�� ���ƺ��� ��
                    transform.rotation = Quaternion.Euler(0, Mathf.Abs(targetPos.rotation.eulerAngles.y - 180), 0);
                    isOverturn = true;
                }
                // �÷��̾�� ���� ���� ������ ���� ���� ������
                else
                {
                    isOverturn = false;
                }

                // ���� �������� ������
                if (isOverturn)
                {
                    // Sprite Renderer�� flip.X�� �ؼ� �÷��̾ �ٶ��� �ʰ� ���� �ִϸ��̼� ������Ѿ� ��
                    myPlayer.Play(surprisedClip, idleClip, true, false);
                }
                // ���� �������� ���� ������
                else
                {
                    // Sprite Renderer�� flip.X�� ���� �ʰ� ���� �ִϸ��̼� ������Ѿ� ��
                    // flip.X�� true�� �ϸ� �ڵ��Ƽ� ���� ����ġ�� �ٽ� ���� ��
                    myPlayer.Play(surprisedClip, idleClip, false, false);
                }

                surprised.SetActive(true);
                isHome = false;
            }
            if(myPlayer.isEndofFrame)
            {
                surprised.SetActive(false);
                // �÷��̾� ����ٴϴ� ���� ����
                MovePosition(targetPos.position.x);
            }

        }
        else
        {
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
        // ��Ÿ�� �־�� ��

        targetPos = player.transform;
        MovePosition(targetPos.position.x);

        if (attackPos.position.y > transform.position.y)
        {
            //UpAttack();
            Debug.Log("���� ����");
        }
        // �÷��̾�� ���� ���� �����ε�
        else if (attackPos.rotation.eulerAngles.y == targetPos.rotation.eulerAngles.y)
        {
            //BackAttack();
            Debug.Log("�Ĺ� ����");
        }

        else if (attackPos.rotation.eulerAngles.y != targetPos.rotation.eulerAngles.y)
        {
            //FrontAttack();
            Debug.Log("���� ����");
        }
    }

    void FrontAttack()
    {
        //myAnimator.SetInteger("AttackNum", 1);
        // ���ط� �߰� �ʿ�

        Debug.Log("���� ����");
    }

    void BackAttack()
    {
        myAnimator.SetBool("isBackAttack", true);
        // ���ط� �߰� �ʿ�

        // �ڽ� Collider enabled �ѱ�
        boxCollider.enabled = true;

        Debug.Log("�Ĺ� ����");
        // ���� ������ �ڽ� Collider enabled ����
    }

    void UpAttack()
    {
        myAnimator.SetBool("isUpAttack", true);
        // ���ط� �߰� �ʿ�

        // ĸ�� Collider Offset, Size ���� �ʿ�
        damageCollider.offset = new Vector2(-0.11f, 0.34f);
        damageCollider.size = new Vector2(3.34f, 4.4f);

        Debug.Log("���� ����");
        // ���� ������ ���� ĸ�� Collider Offset, Size�� ���ƿ���
    }
}
