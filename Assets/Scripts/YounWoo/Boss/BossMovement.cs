using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : Runner, IHittable
{
    [Header("애니메이션 클립")]
    [SerializeField] AnimationClip idleClip;
    [SerializeField] AnimationClip hitClip;
    [SerializeField] AnimationClip stunClip;
    [SerializeField] AnimationClip deathClip;

    [Header("정보")]
    [SerializeField] float HP = 1300;
    [SerializeField] float attackCoolTime;
    [SerializeField] float moveSpeed;

    Collider2D bossCollider;
    Rigidbody2D myRigid;
    AnimatorPlayer myPlayer;

    // 맵의 중간 지점을 박아놓고 오른쪽 왼쪽 판단
    Vector2 midPoint;
    Vector2 targetPos;
    Vector2 pointPos;

    float attackElapsedTime;
    float fullHp;

    bool isStun;

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
        midPoint = GameObject.Find("MidPoint").transform.position;

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
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (target == null)
            {
                Debug.Log("target null");
                Collider2D col = player.GetComponent<Collider2D>();
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

    // 내가 공격하는 대상에 대한 위치 정보 받아서 이동 -> 공격
    public void MoveToAttack(IHittable hitTarget)
    {
        Vector2 direction = Vector2.zero;

        // 때릴 상대에 대한 위치 저장
        targetPos = hitTarget.transform.position;
        
        // 내 위치가 중간 지점보다 크면
        if (transform.position.x > midPoint.x)
        {
            // 중간 지점 왼쪽에 있는 곳이 목표
            pointPos = new Vector2(-(midPoint.x * 2), 0);
            direction = (Vector2)transform.position - pointPos;
            // 밑으로 날아가면서 이동 (대각선으로 내려와서 플레이어 위치로 이동)
            // 내 위치가 때릴 상대보다 위에 있으면
            //if (transform.position.y > targetPos.y)
            //{
            //    // 애니메이션 추가
            //    //myRigid.velocity = new Vector2(-moveSpeed, -moveSpeed);
            //    direction = (Vector2)transform.position - pointPos;
            //    myRigid.velocity -= direction;
            //    Debug.Log("오른쪽 아래 대각선 이동");
            //}
            //else
            //{
            //    myRigid.velocity = new Vector2(-moveSpeed, moveSpeed);
            //    Debug.Log("왼쪽 위 대각선 이동");
            //}
            // 대각선으로 내려와서 내 위치 왼쪽에 때릴 상대가 있으면 왼쪽으로 이동

            // 대각선으로 내려와서 내 위치 오른쪽에 때릴 상대가 있으면 오른쪽 이동
        }
        // 내 위치가 중간 지점보다 작으면
        else if (transform.position.x < midPoint.x)
        {
            // 중간 지점 오른쪽에 있는 곳이 목표
            pointPos = new Vector2((midPoint.x * 2), 0);
            Debug.Log(midPoint.x * 2);
            direction = (Vector2)transform.position - pointPos;
            //if (transform.position.y > targetPos.y)
            //{
            //    myRigid.velocity = new Vector2(moveSpeed, -moveSpeed);
            //    Debug.Log("왼쪽 아래 대각선 이동");
            //}
            //else
            //{
            //    myRigid.velocity = new Vector2(moveSpeed, moveSpeed);
            //    Debug.Log("오른쪽 위 대각선 이동");
            //}
            //direction = (Vector2)transform.position - pointPos;
            //myRigid.velocity -= direction;
        }
        if(transform.position.y > 0)
        {
            myRigid.velocity -= direction;
        }
        else
        {
            myRigid.velocity = Vector2.zero;
        }
    }

    // 공격 -> 애니메이션, 대쉬하는 효과
    public void PlayerDash()
    {

    }
}
