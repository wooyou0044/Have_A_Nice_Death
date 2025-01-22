using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossMovement : Runner, IHittable
{
    [Header("�ִϸ��̼� Ŭ��")]
    [SerializeField] AnimationClip surprisedClip;
    [SerializeField] AnimationClip fightStartClip;
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

    [Header("����")]
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
    GameObject dialogue;

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
    float maxHp;

    bool isStun;
    bool isAttacking;
    bool isMeetPlayer;

    // �ӽ�
    GameObject player;

    //ü�¹�
    //[SerializeField]Boss_Hp_Bar hpBar;
    // �ִ� ü��
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

    // ���� ü��

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

    public bool IsMeetPlayer
    {
        get
        {
            return isMeetPlayer;
        }
        set
        {
            isMeetPlayer = value;
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
        fullHp = maxHp = HP;
        isStun = false;
        isMeetPlayer = false;
    }

    IHittable target;

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // �ο� ������ Ȯ��
        if (isAttacking)
        {
            attackElapsedTime += Time.deltaTime;
            if (bossAI._rechargeTime <= attackElapsedTime)
            {
                attackElapsedTime = 0;
                // ��ų ������̶�� �� ��ȯ
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
            // HP ����
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
    public bool IsAttacking()
    {
        //attackElapsedTime += Time.deltaTime;

        //// attackCoolTime�� ���� ���� �����ϸ�
        //if (bossAI._rechargeTime <= attackElapsedTime)
        //{
        //    attackElapsedTime = 0;
        //    // ��ų ������̶�� �� ��ȯ
        //    return true;
        //}
        //else
        //{
        //    // ��ų ����� �ƴ϶�� �� ��ȯ
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

    // �����ϸ� �� �����̰� �ϴ� �Լ� 
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

    // �밢������ �������� �Լ�
    public void DropDiagonalMove()
    {
        myPlayer.Play(dashLoopClip);
        // �� ��ġ�� �߰� �������� ũ��
        if (transform.position.x >= midPoint.x)
        {
            // ���� ���� �ִ� ���� ��ǥ
            pointPos = startPoint;
            oppositePointX = endPoint.x;
            OnDrawGizmos();
        }
        // �� ��ġ�� �߰� �������� ������
        else
        {
            // ������ ���� �ִ� ���� ��ǥ
            pointPos = endPoint;
            oppositePointX = startPoint.x;
            OnDrawGizmos();
        }

        destination = (Vector2)transform.position - pointPos;
        myRigid.velocity = -destination * (dropSpeed * 0.3f);
    }

    // �������� ���󰡴� �Լ�
    public void FlyMove()
    {
        if (bossAI._skill == GargoyleBrain.Skill.Dash)
        {
            myPlayer.Play(dashLoopClip);
        }

        // �� ��ġ�� �߰� �������� �����ʿ� ������
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

    // ���� ���ƿͼ� �ݴ������� �̵��ϴ� �Լ�
    public void MoveOppositeEndPoint()
    {
        MovePosition(oppositePointX);
    }
    /// <summary>
    /// ���� �÷��̾� ��ġ�� �־�� �ϴ� �÷��̾� ���󰡴� �Լ�
    /// </summary>
    /// <param name="playerPosX"> �÷��̾� ��ġ</param>
    // �÷��̾� ����ٴϸ鼭 �̵��ϴ� �Լ�
    public void FollowPlayer(float playerPosX)
    {
        MovePosition(playerPosX);
    }

   /// <summary>
   /// �÷��̾� ��ġ�� �̹� ����ִ� �÷��̾� ���� ���� �Լ�
   /// </summary>
    public void FollowPlayer()
    {
        MovePosition(player.transform.position.x);
    }

    // �ӽ�
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
        // ���� ��� ������ �߰� �ʿ�(maxHeight / 2)
        pointPos = new Vector2(transform.position.x, maxHeight / 2);
        OnDrawGizmos();

        destination = pointPos - (Vector2)transform.position;
        myRigid.velocity = destination * (flySpeed * 0.2f);

        myPlayer.Play(comboAttack2Clip, idleClip, false);
    }

    // �߰����� �ö󰡴� �Լ� �ʿ�
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


    public void PlayerEnterBossStage()
    {
        myPlayer.Play(surprisedClip,idleClip, false);
        isMeetPlayer = true;
    }

    public bool IsEndSurprised()
    {
        if(myPlayer.isEndofFrame)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FightParticipation()
    {
        myPlayer.Play(fightStartClip, idleClip, false);

        pointPos = new Vector2(endPoint.x, transform.position.y);
        OnDrawGizmos();

        destination = pointPos - (Vector2)transform.position;
        myRigid.velocity = destination * 0.8f;
    }
}
