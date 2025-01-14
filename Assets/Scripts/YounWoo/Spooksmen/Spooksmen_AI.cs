using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Spooksmen_AI : Walker
{
    // ���� ����
    // 1. (����) ��(Attack01) - ������(Attack02) - ����(Attack03) ������ ���濡 �ָ��� �ֵθ� -> ���ط� 12 - 10 - 11
    // 2. (�Ĺ�) ���� ������ ȸ����Ű�� �յ��� ª�� �Ÿ��� ����(Attack04) -> ���ط� 10
    // 3. (����) �������� ������.(Attack02) -> ���ط� 10

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

    Animator myAnimator;
    AnimatorPlayer myPlayer;

    Transform targetPos;
    // ������� ��ġ
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
        // ��� ����Ʈ ����
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

    // �÷��̾� �����ϴ� �Լ� => �����ϴ� �͵� ������ ��
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
            // �� �ڸ��� �ƴϸ� �ٽ� ���ư���
            if(createdPos.x != transform.position.x)
            {
                // ���ƿ����� �ڸ� ���� ���ƴٴϱ�
                if(Mathf.Abs(createdPos.x - transform.position.x) < homeDistance)
                {
                    transform.position = new Vector2(createdPos.x, transform.position.y);
                    // ������ ��ġ�� ���ư��� ���ƴٴϱ� ���� ��ġ ����
                    destination = new Vector2(transform.position.x + wonderRange, transform.position.y);
                    isHome = true;
                }
                else
                {
                    //ComeBackCreatedPosition();
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

    // �÷��̾ �����Ǹ� ����ٴϱ�
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

    // �÷��̾ ���� ������ ����� �� �ڸ��� ���ƿ���
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

    // �÷��̾� ������ ���� ���ƴٴϱ�
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

    // ����
    void Attack()
    {

    }

}
