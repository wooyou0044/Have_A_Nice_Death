using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BossMovement : Runner, IHittable
{
    [Header("애니메이션 클립")]
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

    [Header("정보")]
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

    // 맵의 중간 지점을 박아놓고 오른쪽 왼쪽 판단
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

    // 임시
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

        // 임시
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
            // HP 감소
            HP += strike.result;

            if (isStun == true)
            {
                // 스턴을 일정 시간동안 계속 재생
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

            // 스턴
            if (HP <= fullHp / 2)
            {
                isStun = true;
                myPlayer.Play(stunStartClip, stunIdleClip, false);
            }

            if (isStun == false)
            {
                myPlayer.Play(hitClip, idleClip, false);
            }
            // 만약에 나랑 같은 방향을 바라보고 있으면 턴 해야 함
        }
        else
        {
            if (bossCollider.isActiveAndEnabled == true)
            {
                HP = 0;
                bossCollider.enabled = false;
                myRigid.gravityScale = 0;
                // gameObject는 아직 죽이면 안 됨 => 대화 창 켜지고 새로운 애로 바뀌어야 함
                myPlayer.Play(deathClip);
            }
        }
    }

    // 스킬을 사용중인가에 대한 함수
    public bool isAttacking()
    {
        attackElapsedTime += Time.deltaTime;

        // attackCoolTime이 차서 공격 실행하면
        if (attackCoolTime <= attackElapsedTime)
        {
            attackElapsedTime = 0;
            // 스킬 사용중이라는 값 반환
            return true;
        }
        else
        {
            // 스킬 사용중 아니라는 값 반환
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

    // 도착하면 안 움직이게 하는 함수 
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

    // 대각선으로 떨어지는 함수
    public void DropDiagonalMove()
    {
        myPlayer.Play(dashLoopClip);
        // 내 위치가 중간 지점보다 크면
        if (transform.position.x > midPoint.x)
        {
            // 왼쪽 끝에 있는 곳이 목표
            pointPos = startPoint;
            oppositePointX = endPoint.x;
            OnDrawGizmos();

            // 애니메이션 추가 필요

            destination = (Vector2)transform.position - pointPos;
            myRigid.velocity = -destination * (dropSpeed * 0.3f);
        }
        // 내 위치가 중간 지점보다 작으면
        else if (transform.position.x < midPoint.x)
        {
            // 오른쪽 끝에 있는 곳이 목표
            pointPos = endPoint;
            oppositePointX = startPoint.x;
            OnDrawGizmos();

            // 애니메이션 추가 필요

            destination = (Vector2)transform.position - pointPos;
            myRigid.velocity = -destination * (dropSpeed * 0.3f);

        }
    }

    // 공중으로 날라가는 함수
    public void FlyMove()
    {
        if (bossAI._skill == GargoyleBrain.Skill.Dash)
        {
            myPlayer.Play(dashLoopClip);
        }

        Vector2 flyDestination = Vector2.zero;
        // 내 위치가 중간 지점보다 오른쪽에 있으면
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

    // 내려 날아와서 반대쪽으로 이동하는 함수
    public void MoveOppositeEndPoint()
    {
        MovePosition(oppositePointX);
    }

    // 플레이어 따라다니면서 이동하는 함수
    public void FollowPlayer(float playerPosX)
    {
        UTurn();
        MovePosition(playerPosX);
    }

    // 임시
    new void OnDrawGizmos()
    {
        Debug.DrawRay(pointPos, (Vector2)transform.position - pointPos);
    }

    public void ComboAttack()
    {

    }

    // 임시 테스트용
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "tempBox")
        {
            Debug.Log("부딪힘");
            isBox = true;
        }
    }
}
