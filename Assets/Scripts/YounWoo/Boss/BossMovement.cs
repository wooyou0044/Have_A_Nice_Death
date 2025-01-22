using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BossMovement : Runner, IHittable
{
    [Header("�ִϸ��̼� Ŭ��")]
    [SerializeField] AnimationClip idleClip;
    [SerializeField] AnimationClip uTurnClip;
    [SerializeField] AnimationClip hitClip;
    [SerializeField] AnimationClip stunStartClip;
    [SerializeField] AnimationClip stunIdleClip;
    [SerializeField] AnimationClip stunEndClip;
    [SerializeField] AnimationClip dashStartClip;
    [SerializeField] AnimationClip dashLoopClip;
    [SerializeField] AnimationClip dashEndClip;
    [SerializeField] AnimationClip deathClip;

    [Header("����")]
    [SerializeField] float HP = 1300;
    [SerializeField] float attackCoolTime;
    [SerializeField] float dropSpeed;
    [SerializeField] float flySpeed;
    [SerializeField] float maxHeight;
    [SerializeField] GameObject stunPrefab;
    [SerializeField] Transform stunPos;

    Collider2D bossCollider;
    Rigidbody2D myRigid;
    AnimatorPlayer myPlayer;
    GargoyleBrain bossAI;

    GameObject stun;

    // ���� �߰� ������ �ھƳ��� ������ ���� �Ǵ�
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 midPoint;
    Vector2 targetPos;
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
        bossAI = GetComponent<GargoyleBrain>();

        // �ӽ�
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        stun = Instantiate(stunPrefab, stunPos);
        stun.transform.localScale = new Vector2(0.7f, 0.7f);
        stun.SetActive(false);
        midPoint = (startPoint + endPoint) / 2;
        fullHp = HP;
        isStun = false;
    }

    IHittable target;

    void Update()
    {
    }

    public Collider2D GetCollider2D()
    {
        return bossCollider;
    }

    public void Hit(Strike strike)
    {
        Debug.Log(HP);
        if (isAlive)
        {
            // HP ����
            HP += strike.result;

            if (isStun == true)
            {
                // ������ ���� �ð����� ��� ���
                myPlayer.Play(stunIdleClip);
                stun.SetActive(true);
                fullHp = 0;
                StartCoroutine(DoStun());
                IEnumerator DoStun()
                {
                    yield return new WaitForSeconds(1.2f);
                    myPlayer.Play(stunEndClip, idleClip, false);
                    stun.SetActive(false);
                    isStun = false;
                }
            }

            // ����
            if (HP <= fullHp / 2)
            {
                isStun = true;
                myPlayer.Play(stunStartClip, stunIdleClip, false);
            }

            if (isStun == false)
            {
                myPlayer.Play(hitClip, idleClip, false);
            }
            // ���࿡ ���� ���� ������ �ٶ󺸰� ������ �� �ؾ� ��
        }
        else
        {
            if (bossCollider.isActiveAndEnabled == true)
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
        if (attackCoolTime <= attackElapsedTime)
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
        if (bossAI._skill == GargoyleBrain.Skill.Dash)
        {
            myPlayer.Play(dashLoopClip);
        }

        if (transform.position.x < targetPosX)
        {
            MoveRight();
        }
        else if (transform.position.x > targetPosX)
        {
            MoveLeft();
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

    public void UTurn()
    {
        if (transform.position.x > midPoint.x)
        {
            if (transform.rotation.eulerAngles.y <= 0)
            {
                MoveStop();
                if (bossAI._skill == GargoyleBrain.Skill.Dash)
                {
                    myPlayer.Play(dashStartClip);
                }
                else
                {
                    myPlayer.Play(uTurnClip, idleClip, true);
                }
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }
        }
        else
        {
            if (transform.rotation.eulerAngles.y >= 180)
            {
                MoveStop();
                if (bossAI._skill == GargoyleBrain.Skill.Dash)
                {
                    myPlayer.Play(dashStartClip);
                }
                else
                {
                    myPlayer.Play(uTurnClip, idleClip, true);
                }
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    // �밢������ �������� �Լ�
    public void DropDiagonalMove()
    {
        myPlayer.Play(dashLoopClip);
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
        if (bossAI._skill == GargoyleBrain.Skill.Dash)
        {
            myPlayer.Play(dashLoopClip);
        }

        Vector2 flyDestination = Vector2.zero;
        // �� ��ġ�� �߰� �������� �����ʿ� ������
        if (transform.position.x >= midPoint.x)
        {
            pointPos = new Vector2(startPoint.x, maxHeight);
            OnDrawGizmos();

            destination = pointPos - (Vector2)transform.position;
            myRigid.velocity = destination * (flySpeed * 0.3f);
        }
        else if (transform.position.x < midPoint.x)
        {
            pointPos = new Vector2(endPoint.x, maxHeight);
            OnDrawGizmos();

            destination = pointPos - (Vector2)transform.position;
            myRigid.velocity = destination * (flySpeed * 0.3f);
        }
    }

    // ���� ���ƿͼ� �ݴ������� �̵��ϴ� �Լ�
    public void MoveOppositeEndPoint()
    {
        MovePosition(oppositePointX);
    }

    // �÷��̾� ����ٴϸ鼭 �̵��ϴ� �Լ�
    public void FollowPlayer(float playerPosX)
    {
        UTurn();
        MovePosition(playerPosX);
    }

    // �ӽ�
    new void OnDrawGizmos()
    {
        Debug.DrawRay(pointPos, (Vector2)transform.position - pointPos);
    }

    public void ComboAttack()
    {

    }

    // �ӽ� �׽�Ʈ��
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "tempBox")
        {
            Debug.Log("�ε���");
            isBox = true;
        }
    }
}
