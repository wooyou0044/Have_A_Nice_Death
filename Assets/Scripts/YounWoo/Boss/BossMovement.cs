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
    Vector2 midPoint;
    Vector2 targetPos;

    float attackElapsedTime;
    float fullHp;

    bool isStun;

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
        midPoint = GameObject.Find("MidPoint").transform.position;
    }

    private void Start()
    {
        fullHp = HP;
        isStun = false;
    }

    IHittable target;

    void Update()
    {
        // �ӽ�
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (target == null)
            {
                Collider2D col = Physics2D.OverlapCircle(transform.position, 5.0f);
                target = col.GetComponent<IHittable>();
            }
            else
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
        // ���� ��뿡 ���� ��ġ ����
        targetPos = hitTarget.transform.position;

        // �� ��ġ�� ���� ��뺸�� ���� ������
        if(transform.position.y > targetPos.y)
        {
            // ������ ���ư��鼭 �̵� (�밢������ �����ͼ� �÷��̾� ��ġ�� �̵�)
            // �� ��ġ�� ������ ����
            if(transform.position.x > midPoint.x)
            {
                // �ִϸ��̼� �߰�
                myRigid.velocity = new Vector2(-moveSpeed, -moveSpeed);
            }
            else
            {
                myRigid.velocity = new Vector2(moveSpeed, -moveSpeed);
            }

            // �밢������ �����ͼ� �� ��ġ ���ʿ� ���� ��밡 ������ �������� �̵�

            // �밢������ �����ͼ� �� ��ġ �����ʿ� ���� ��밡 ������ ������ �̵�
        }

        // �� ��ġ�� ���� ��뺸�� �Ʒ��� ������
        else if(transform.position.y < targetPos.y)
        {
            // ���� ���ư��鼭 �̵� (�밢������ �ö󰡼� �÷��̾� ��ġ�� �̵�)
            if(transform.position.x > midPoint.x)
            {
                myRigid.velocity = new Vector2(-moveSpeed, moveSpeed);
            }

            // �� ��ġ ���ʿ� ���� ��밡 ������ �������� �̵�
            else
            {
                myRigid.velocity = new Vector2(moveSpeed, moveSpeed);
            }

            // �� ��ġ �����ʿ� ���� ��밡 ������ ������ �̵�
        }

        // ���� => �ִϸ��̼�, �뽬�ϴ� ȿ��
    }

    // ���� -> �ִϸ��̼�, �뽬�ϴ� ȿ��
    public void PlayerDash()
    {

    }
}
