using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : Runner, IHittable
{
    [Header("�ִϸ��̼� Ŭ��")]
    [SerializeField] AnimationClip idleClip;
    [SerializeField] AnimationClip hitClip;
    [SerializeField] AnimationClip stunClip;
    [SerializeField] AnimationClip deathClip;

    [Header("����")]
    [SerializeField] float HP = 1300;
    [SerializeField] float attackCoolTime;
    [SerializeField] float moveSpeed;

    Collider2D bossCollider;
    Rigidbody2D myRigid;
    AnimatorPlayer myPlayer;

    // ���� �߰� ������ �ھƳ��� ������ ���� �Ǵ�
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 midPoint;
    Vector2 targetPos;
    Vector2 pointPos;

    float attackElapsedTime;
    float fullHp;

    bool isStun;
    bool isArrive;

    // �ӽ�
    GameObject player;

    public bool isAlive
    {
        get
        {
            return (HP > 0) ? true : false;
        }
    }

    void Awake()
    {
        bossCollider = GetComponent<Collider2D>();
        myPlayer = GetComponent<AnimatorPlayer>();
        myRigid = GetComponent<Rigidbody2D>();
        startPoint = GameObject.Find("StartPoint").transform.position;
        endPoint = GameObject.Find("EndPoint").transform.position;

        midPoint = (startPoint + endPoint) / 2;

        // �ӽ�
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        fullHp = HP;
        isStun = false;
    }

    IHittable target;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            // �ӽ�
            if (target == null)
            {
                Debug.Log("target null");
                Collider2D col = player.GetComponent<Collider2D>();
                target = col.GetComponent<IHittable>();
            }
            else if (isArrive == false)
            {
                Debug.Log("target");
                MoveToAttack(target);
            }
        }
    }

    public Collider2D GetCollider2D()
    {
        return bossCollider;
    }

    public void Hit(Strike strike)
    {
        Debug.Log(HP);
        if(isAlive)
        {
            // HP ����
            HP += strike.result;

            //if(isStun == true)
            //{
            //    // ������ ���� �ð����� ��� ���
            //    myPlayer.Play(stunClip);
            //    StartCoroutine(DoStun());
            //    IEnumerator DoStun()
            //    {
            //        yield return new WaitForSeconds(5f);
            //        isStun = false;
            //        fullHp = 0;
            //    }
            //    //isStun = false;
            //    //fullHp = 0;
            //}

            // ����
            if(HP <= fullHp / 2)
            {
                isStun = true;
            }

            if(isStun == false)
            {
                myPlayer.Play(hitClip, idleClip, false);
            }
            // ���࿡ ���� ���� ������ �ٶ󺸰� ������ �� �ؾ� ��
        }
        else
        {
            if(bossCollider.isActiveAndEnabled == true)
            {
                HP = 0;
                bossCollider.enabled = false;
                myRigid.gravityScale = 0;
                // gameObject�� ���� ���̸� �� �� => ��ȭ â ������ ���ο� �ַ� �ٲ��� ��
                myPlayer.Play(deathClip);
            }
        }
    }

    // ��ų�� ������ΰ��� ���� �Լ�
    public bool isAttacking()
    {
        attackElapsedTime += Time.deltaTime;

        // attackCoolTime�� ���� ���� �����ϸ�
        if(attackCoolTime <= attackElapsedTime)
        {
            attackElapsedTime = 0;
            // ��ų ������̶�� �� ��ȯ
            return true;
        }
        else
        {
            // ��ų ����� �ƴ϶�� �� ��ȯ
            return false;
        }
    }

    // ���� �����ϴ� ��� ���� ��ġ ���� �޾Ƽ� �̵� -> ����
    public void MoveToAttack(IHittable hitTarget)
    {
        // �ӽ�
        isArrive = false;

        Vector2 destination = Vector2.zero;
        // ���� ��뿡 ���� ��ġ ����
        targetPos = hitTarget.transform.position;
        
        // �� ��ġ�� �߰� �������� ũ��
        if (transform.position.x > midPoint.x)
        {
            // ���� ���� �ִ� ���� ��ǥ
            pointPos = startPoint;
            OnDrawGizmos();
            // ������ ���ư��鼭 �̵� (�밢������ �����ͼ� �÷��̾� ��ġ�� �̵�)
            // �� ��ġ�� ���� ��뺸�� ���� ������
            if (transform.position.y > targetPos.y)
            {
                // �ִϸ��̼� �߰�
                destination = (Vector2)transform.position - pointPos;
                //myRigid.velocity = new Vector2(-moveSpeed, -moveSpeed);
                Debug.Log("������ �Ʒ� �밢�� �̵�");
            }
            else
            {
                //myRigid.velocity = new Vector2(-moveSpeed, moveSpeed);
                Debug.Log("���� �� �밢�� �̵�");
            }

            myRigid.velocity -= destination * (moveSpeed * 0.4f);
            // �밢������ �����ͼ� �� ��ġ ���ʿ� ���� ��밡 ������ �������� �̵�

            // �밢������ �����ͼ� �� ��ġ �����ʿ� ���� ��밡 ������ ������ �̵�
        }
        // �� ��ġ�� �߰� �������� ������
        else if (transform.position.x < midPoint.x)
        {
            // ������ ���� �ִ� ���� ��ǥ
            pointPos = endPoint;
            OnDrawGizmos();
            if (transform.position.y > targetPos.y)
            {
                destination = (Vector2)transform.position - pointPos;
                //myRigid.velocity = new Vector2(moveSpeed, -moveSpeed);
                Debug.Log("���� �Ʒ� �밢�� �̵�");
            }
            else
            {
                //myRigid.velocity = new Vector2(moveSpeed, moveSpeed);
                Debug.Log("������ �� �밢�� �̵�");
            }
            myRigid.velocity -= destination * (moveSpeed * 0.4f);
        }

        if (myRigid.velocity.y == 0)
        {
            Debug.Log("����");
            //myRigid.velocity = new Vector2(0, 0);
            //// �ӽ�
            //isArrive = true;
        }
    }

    // �ӽ�
    new void OnDrawGizmos()
    {
        Debug.DrawRay(pointPos, (Vector2)transform.position - pointPos);
    }

    // ���� -> �ִϸ��̼�, �뽬�ϴ� ȿ��
    public void PlayerDash()
    {

    }
}
