using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossMovement : Runner, IHittable
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
    [SerializeField] AnimationClip comboAttack1Clip;
    [SerializeField] AnimationClip comboAttack2Clip;
    [SerializeField] AnimationClip deathClip;

    [Header("정보")]
    [SerializeField] float HP = 1300;
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
    float maxHp;

    bool isStun;
    bool isAttacking;

    // 임시
    GameObject player;
    bool isEndPoint;
    bool isBox;

    //체력바
    //[SerializeField]Boss_Hp_Bar hpBar;
    // 최대 체력
    public float MaxHP
    {
        get
        {
            return maxHp;
        }
    }

    public float currentHP
    {
        get
        {
            return HP;
        }
    }

    // 현재 체력

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
        fullHp = maxHp = HP;
        isStun = false;
    }

    IHittable target;

    void Update()
    {
        // 싸움 중인지 확인
        if(isAttacking)
        {
            attackElapsedTime += Time.deltaTime;
            if (bossAI._rechargeTime <= attackElapsedTime)
            {
                attackElapsedTime = 0;
                // 스킬 사용중이라는 값 반환
                isAttacking = false;
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
        //hpBar.UpdateBossHPBar();
        if (isAlive)
        {
            // HP 감소
            HP += strike.result;
          
            
            if (transform.rotation.eulerAngles.y == player.transform.rotation.eulerAngles.y)
            {
                transform.rotation = Quaternion.Euler(0, -(180 - transform.rotation.eulerAngles.y), 0);
            }

            if (bossAI._skill != GargoyleBrain.Skill.Dash)
            {
            }

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
    public bool IsAttacking()
    {
        //attackElapsedTime += Time.deltaTime;

        //// attackCoolTime이 차서 공격 실행하면
        //if (bossAI._rechargeTime <= attackElapsedTime)
        //{
        //    attackElapsedTime = 0;
        //    // 스킬 사용중이라는 값 반환
        //    return true;
        //}
        //else
        //{
        //    // 스킬 사용중 아니라는 값 반환
        //    return false;
        //}
        return isAttacking;
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
        myPlayer.Play(idleClip);
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
        if (transform.position.x >= midPoint.x)
        {
            // 왼쪽 끝에 있는 곳이 목표
            pointPos = startPoint;
            oppositePointX = endPoint.x;
            OnDrawGizmos();
        }
        // 내 위치가 중간 지점보다 작으면
        else
        {
            // 오른쪽 끝에 있는 곳이 목표
            pointPos = endPoint;
            oppositePointX = startPoint.x;
            OnDrawGizmos();
        }

        destination = (Vector2)transform.position - pointPos;
        myRigid.velocity = -destination * (dropSpeed * 0.3f);
    }

    // 공중으로 날라가는 함수
    public void FlyMove()
    {
        if (bossAI._skill == GargoyleBrain.Skill.Dash)
        {
            myPlayer.Play(dashLoopClip);
        }

        // 내 위치가 중간 지점보다 오른쪽에 있으면
        if (transform.position.x >= midPoint.x)
        {
            pointPos = new Vector2(startPoint.x, maxHeight);
            OnDrawGizmos();
        }

        else if (transform.position.x < midPoint.x)
        {
            pointPos = new Vector2(endPoint.x, maxHeight);
            OnDrawGizmos();
        }

        destination = pointPos - (Vector2)transform.position;
        myRigid.velocity = destination * (flySpeed * 0.3f);
    }

    // 내려 날아와서 반대쪽으로 이동하는 함수
    public void MoveOppositeEndPoint()
    {
        MovePosition(oppositePointX);
    }
    /// <summary>
    /// 내가 플레이어 위치를 넣어야 하는 플레이어 따라가는 함수
    /// </summary>
    /// <param name="playerPosX"> 플레이어 위치</param>
    // 플레이어 따라다니면서 이동하는 함수
    public void FollowPlayer(float playerPosX)
    {
        MovePosition(playerPosX);
    }

   /// <summary>
   /// 플레이어 위치가 이미 들어있는 플레이어 따라 가는 함수
   /// </summary>
    public void FollowPlayer()
    {
        MovePosition(player.transform.position.x);
    }

    // 임시
    new void OnDrawGizmos()
    {
        Debug.DrawRay(pointPos, (Vector2)transform.position - pointPos);
    }

    public void ComboAttack1()
    {
        isAttacking = true;
        myPlayer.Play(comboAttack1Clip, idleClip, false);
    }

    public void ComboAttack2()
    {
        isAttacking = true;
        // 위로 잠깐 떠오름 추가 필요(maxHeight / 2)
        pointPos = new Vector2(transform.position.x, maxHeight / 2);
        OnDrawGizmos();

        destination = pointPos - (Vector2)transform.position;
        myRigid.velocity = destination * (flySpeed * 0.2f);

        myPlayer.Play(comboAttack2Clip, idleClip, false);
    }

    // 중간으로 올라가는 함수 필요
    public void MoveMiddleSpot()
    {
        pointPos = new Vector2(midPoint.x, maxHeight);
        OnDrawGizmos();

        destination = pointPos - (Vector2)transform.position;
        myRigid.velocity = destination * (flySpeed * 0.3f);
    }

    [SerializeField]
    private Projectile _dashProjectile;

    public void AdjustRotation()
    {
        Vector2 velocity = getRigidbody2D.velocity;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        if (velocity.x >= 0)
        {
            getTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            getTransform.rotation = Quaternion.Euler(180, 0, -angle);
        }
    }

    public void UseDashSkill(float duration)
    {
        Projectile projectile = GameManager.GetProjectile(_dashProjectile);
        projectile.Shot(getTransform, null, GameManager.ShowEffect, GameManager.Use, null, duration);
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
