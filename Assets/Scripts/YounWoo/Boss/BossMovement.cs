using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : Runner, IHittable
{
    [Header("�ִϸ��̼� Ŭ��")]
    [SerializeField] AnimationClip idleClip;
    [SerializeField] AnimationClip uTurnClip;
    [SerializeField] AnimationClip hitClip;
    [SerializeField] AnimationClip stunClip;
    [SerializeField] AnimationClip deathClip;

    [Header("����")]
    [SerializeField] float HP = 1300;
    [SerializeField] float attackCoolTime;
    [SerializeField] float dropSpeed;
    [SerializeField] float flySpeed;
    [SerializeField] float maxHeight;

    Collider2D bossCollider;
    Rigidbody2D myRigid;
    AnimatorPlayer myPlayer;

    // ���� �߰� ������ �ھƳ��� ������ ���� �Ǵ�
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 midPoint;
    //Vector2 targetPos;
    Vector2 pointPos;
    Vector2 destination;

    float attackElapsedTime;
    float fullHp;
    float oppositePointX;

    bool isStun;
    bool isArrive;

    // �ӽ�
    GameObject player;
    bool isEndPoint;
    bool isBox;

    public float StartPointX
    {
        get { return startPoint.x; }
    }

    public float EndPointX
    {
        get { return endPoint.x; }
    }

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
        // �ӽ�
        if(isGrounded)
        {
            isArrive = true;

            //MovePosition(targetPos.x);
            // �ӽ÷�
            if(isEndPoint == false)
            {
                //MoveOppositeEndPoint();
                //FollowPlayer(targetPos.x);
            }
        }

        if(isBox == true)
        {
            myRigid.velocity = new Vector2(0, 0);
            isBox = false;
        }

        // �ӽ�
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (target == null)
            {
                Debug.Log("target null");
                Collider2D col = player.GetComponent<Collider2D>();
                target = col.GetComponent<IHittable>();
            }
            else if (isArrive == false)
            {
                Debug.Log("target");
                //MoveToAttack(target);
                DropDiagonalMove();
                //FlyMove();
            }
            else
            {
                
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

    public void MovePosition(float targetPosX)
    {
        if(transform.position.x < targetPosX)
        {
            MoveRight();
        }
        else if(transform.position.x > targetPosX)
        {
            MoveLeft();
        }
    }

    public override void MoveLeft()
    {
        if(transform.rotation.eulerAngles.y <= 0)
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


    // �����ϸ� �� �����̰� �ϴ� �Լ� 
    public override void MoveStop()
    {
        if (myRigid.velocity.y != 0)
        {
            myRigid.velocity = new Vector2(0, 0);
        }
        else
        {
            base.MoveStop();
        }
    }

    void UTurn()
    {

    }

    // �밢������ �������� �Լ�
    public void DropDiagonalMove()
    {
        Debug.Log("�밢�� �̵�");
        // �� ��ġ�� �߰� �������� ũ��
        if (transform.position.x > midPoint.x)
        {
            // ���� ���� �ִ� ���� ��ǥ
            pointPos = startPoint;
            oppositePointX = endPoint.x;
            OnDrawGizmos();

            // �ִϸ��̼� �߰� �ʿ�

            destination = (Vector2)transform.position - pointPos;
            myRigid.velocity = -destination * (dropSpeed * 0.3f);
        }
        // �� ��ġ�� �߰� �������� ������
        else if (transform.position.x < midPoint.x)
        {
            // ������ ���� �ִ� ���� ��ǥ
            pointPos = endPoint;
            oppositePointX = startPoint.x;
            OnDrawGizmos();

            // �ִϸ��̼� �߰� �ʿ�

            destination = (Vector2)transform.position - pointPos;
            myRigid.velocity = -destination * (dropSpeed * 0.3f);

        }
    }

    // �������� ���󰡴� �Լ�
    public void FlyMove()
    {
        Vector2 flyDestination = Vector2.zero;

        // �� ��ġ�� �߰� �������� �����ʿ� ������
        if(transform.position.x >= midPoint.x)
        {
            pointPos = new Vector2(startPoint.x, maxHeight);
            OnDrawGizmos();

            destination = pointPos - (Vector2)transform.position;
            myRigid.velocity += destination * (flySpeed * 0.3f);
        }
        else if (transform.position.x < midPoint.x)
        {
            pointPos = new Vector2(endPoint.x, maxHeight);
            OnDrawGizmos();

            destination = pointPos - (Vector2)transform.position;
            myRigid.velocity += destination * (flySpeed * 0.3f);
        }
    }

    // ���� ���ƿͼ� �ݴ������� �̵��ϴ� �Լ�
    public void MoveOppositeEndPoint()
    {
        MovePosition(oppositePointX);
        isEndPoint = true;
    }

    // �÷��̾� ����ٴϸ鼭 �̵��ϴ� �Լ�
    public void FollowPlayer(float playerPosX)
    {
        if(myRigid.constraints == RigidbodyConstraints2D.FreezePosition)
        {
            myRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        MovePosition(playerPosX);
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

    // �ӽ� �׽�Ʈ��
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "tempBox")
        {
            Debug.Log("�ε���");
            isBox = true;
        }
    }
}
