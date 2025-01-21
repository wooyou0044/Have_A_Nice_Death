using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : Runner, IHittable
{
    [Header("애니메이션 클립")]
    [SerializeField] AnimationClip idleClip;
    [SerializeField] AnimationClip uTurnClip;
    [SerializeField] AnimationClip hitClip;
    [SerializeField] AnimationClip stunClip;
    [SerializeField] AnimationClip deathClip;

    [Header("정보")]
    [SerializeField] float HP = 1300;
    [SerializeField] float attackCoolTime;
    [SerializeField] float dropSpeed;
    [SerializeField] float flySpeed;
    [SerializeField] float maxHeight;

    Collider2D bossCollider;
    Rigidbody2D myRigid;
    AnimatorPlayer myPlayer;

    // 맵의 중간 지점을 박아놓고 오른쪽 왼쪽 판단
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

        midPoint = (startPoint + endPoint) / 2;

        // 임시
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
        // 임시
        if(isGrounded)
        {
            isArrive = true;

            //MovePosition(targetPos.x);
            // 임시로
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

        // 임시
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
            // HP 감소
            HP += strike.result;

            //if(isStun == true)
            //{
            //    // 스턴을 일정 시간동안 계속 재생
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

            // 스턴
            if(HP <= fullHp / 2)
            {
                isStun = true;
            }

            if(isStun == false)
            {
                myPlayer.Play(hitClip, idleClip, false);
            }
            // 만약에 나랑 같은 방향을 바라보고 있으면 턴 해야 함
        }
        else
        {
            if(bossCollider.isActiveAndEnabled == true)
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
        if(attackCoolTime <= attackElapsedTime)
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

    void UTurn()
    {

    }

    // 대각선으로 떨어지는 함수
    public void DropDiagonalMove()
    {
        Debug.Log("대각선 이동");
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
        Vector2 flyDestination = Vector2.zero;

        // 내 위치가 중간 지점보다 오른쪽에 있으면
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

    // 내려 날아와서 반대쪽으로 이동하는 함수
    public void MoveOppositeEndPoint()
    {
        MovePosition(oppositePointX);
        isEndPoint = true;
    }

    // 플레이어 따라다니면서 이동하는 함수
    public void FollowPlayer(float playerPosX)
    {
        if(myRigid.constraints == RigidbodyConstraints2D.FreezePosition)
        {
            myRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        MovePosition(playerPosX);
    }

    // 임시
    new void OnDrawGizmos()
    {
        Debug.DrawRay(pointPos, (Vector2)transform.position - pointPos);
    }

    // 공격 -> 애니메이션, 대쉬하는 효과
    public void PlayerDash()
    {

    }

    // 임시 테스트용
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "tempBox")
        {
            Debug.Log("부딪힘");
            isBox = true;
        }
    }
}
