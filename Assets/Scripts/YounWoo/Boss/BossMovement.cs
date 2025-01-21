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

    Collider2D bossCollider;
    Rigidbody2D myRigid;
    AnimatorPlayer myPlayer;

    // 맵의 중간 지점을 박아놓고 오른쪽 왼쪽 판단
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 midPoint;
    Vector2 targetPos;
    Vector2 pointPos;

    float attackElapsedTime;
    float fullHp;
    float maxHeight;

    bool isStun;
    bool isArrive;

    // 임시
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
            Debug.Log("들어옴");
            isArrive = true;

            MovePosition(targetPos.x);
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
                MoveToAttack(target);
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
        if(transform.position.x <= targetPosX)
        {
            MoveRight();
        }
        else
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

    //void UTurn(Transform targetTrans)
    //{
    //    float myRotation = transform.rotation.eulerAngles.y;
    //    if (targetTrans.rotation.eulerAngles.y >= 180)
    //    {
    //        MoveStop();
    //        myPlayer.Play(uTurnClip, idleClip, true);
    //        transform.rotation = Quaternion.Euler(0, 0, 0);
    //    }
    //    else
    //    {

    //    }
    //}

    // 내가 공격하는 대상에 대한 위치 정보 받아서 이동 -> 공격
    public void MoveToAttack(IHittable hitTarget)
    {
        // 임시
        isArrive = false;

        Vector2 destination = Vector2.zero;
        // 때릴 상대에 대한 위치 저장
        targetPos = hitTarget.transform.position;
        
        // 내 위치가 중간 지점보다 크면
        if (transform.position.x > midPoint.x)
        {
            // 왼쪽 끝에 있는 곳이 목표
            pointPos = startPoint;
            OnDrawGizmos();
            // 밑으로 날아가면서 이동 (대각선으로 내려와서 플레이어 위치로 이동)
            // 내 위치가 때릴 상대보다 위에 있으면
            if (transform.position.y > targetPos.y)
            {
                // 애니메이션 추가
                destination = (Vector2)transform.position - pointPos;
                myRigid.velocity -= destination * (dropSpeed * 0.4f);
                //myRigid.velocity = new Vector2(-moveSpeed, -moveSpeed);
                Debug.Log("오른쪽 아래 대각선 이동");
            }
            else
            {
                //myRigid.velocity = new Vector2(-moveSpeed, moveSpeed);
                Debug.Log("왼쪽 위 대각선 이동");
            }
        }
        // 내 위치가 중간 지점보다 작으면
        else if (transform.position.x < midPoint.x)
        {
            // 오른쪽 끝에 있는 곳이 목표
            pointPos = endPoint;
            OnDrawGizmos();
            if (transform.position.y > targetPos.y)
            {
                destination = (Vector2)transform.position - pointPos;
                myRigid.velocity -= destination * (dropSpeed * 0.4f);
                //myRigid.velocity = new Vector2(moveSpeed, -moveSpeed);
                Debug.Log("왼쪽 아래 대각선 이동");
            }
            else
            {
                //myRigid.velocity = new Vector2(moveSpeed, moveSpeed);
                Debug.Log("오른쪽 위 대각선 이동");
            }
        }

        // 대각선으로 내려와서 내 위치 왼쪽에 때릴 상대가 있으면 왼쪽으로 이동
        // 대각선으로 내려와서 내 위치 오른쪽에 때릴 상대가 있으면 오른쪽 이동

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
}
