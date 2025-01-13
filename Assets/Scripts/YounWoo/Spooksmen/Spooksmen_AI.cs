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

    [Header("�÷��̾� ����")]
    // �����ϴ� ����
    [SerializeField] float sightRange;
    // �÷��̾� �������� �� �ѵ� ����ǥ ������Ʈ
    [SerializeField] GameObject surprisedMark;
    [SerializeField] Transform surprisedPos;
    [SerializeField] LayerMask playerMask;

    Animator myAnimator;

    Transform targetPos;
    // ������� ��ġ
    Transform createdPos;

    GameObject surprised;
    // �ӽ�
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
        // ��� ����Ʈ ����
        surprised = Instantiate(surprisedMark, surprisedPos);
        surprised.SetActive(false);
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
            // �÷��̾ �߰ߵ� �� �ѹ��� ���� -> ����ٴϴٰ� �� �ڸ��� ���ư��� ���� ���ƴٴϴٰ� �ٽ� �÷��̾� �߰����� �� ���� (FollowPlayer)
            // �ӽ÷�
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
            // �� �ڸ��� �ƴϸ� �ٽ� ���ư��� �� �ڸ��� ���ƴٴϱ�(ComeBackCreatedPosition, WonderSpotRange)
            //ComeBackCreatedPosition();
            //Debug.Log("createdPos : " + createdPos.position);
            surprised.SetActive(false);
        }
    }

    // �÷��̾ �����Ǹ� ����ٴϱ�
    void FollowPlayer()
    {
        if(transform.position.x <= targetPos.position.x)
        {
            // ���࿡ ���� rotation y ��ǥ�� 180�̸� ����
            if (transform.rotation.eulerAngles.y >= 180)
            {
                // ������ MoveStop();
                MoveStop();
                isRight = true;
                myAnimator.SetBool("isUturn", true);
            }
            else
            {
                //transform.rotation = Quaternion.Euler(0, 0, 0);
                // ���������� �̵�
                MoveRight();
            }
        }
        else
        {
            // ���࿡ ���� rotation y ��ǥ�� 0�̸� ����
            if(transform.rotation.eulerAngles.y <= 0)
            {
                // ������ MoveStop();
                MoveStop();
                isRight = false;
                myAnimator.SetBool("isUturn", true);
            }

            else
            {
                //transform.rotation = Quaternion.Euler(0, 180, 0);
                // �������� �̵�
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

    // �÷��̾ ���� ������ ����� �� �ڸ��� ���ƿ���
    void ComeBackCreatedPosition()
    {
        //if (transform.position.x != createdPos.position.x)
        //{
        //    // ��� ���ƺ������� �ϴ� ���߿� ����
        //    myRigid.velocity = new Vector2(-moveSpeed, 0);
        //}

        // ���ڸ��� ���ƿ��� isSurprised �ѵα�
    }

    // �÷��̾� ������ ���� ���ƴٴϱ�
    void WonderSpotRange()
    {

    }

    // �÷��̾ ���� ������ �̵��� �� Uturn�ϱ�

}
