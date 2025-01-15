using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class WomanGhostAi_Clone : Walker
{
    [SerializeField]
    Projectile womanFire;
    [SerializeField]
    Transform Launcher;

    //�ڽĿ�����Ʈ
    [SerializeField,Header("�ڽĽ�ũ��Ʈ")]
    surpriseImote getSurprisedImote;
    [SerializeField,Header("�ڽľִϸ��̼�")]
    private Animator surprisedAnimator;

    //�ִϸ��̼� Ŭ����
    [SerializeField, Header("�ִϸ��̼� Ŭ��")]
    private AnimatorPlayer animatorPlayer;
    [SerializeField]
    private AnimationClip idleClip;
    [SerializeField]
    private AnimationClip spotteddClip;
    [SerializeField]
    private AnimationClip attackClip;
    [SerializeField]
    private AnimationClip uturnClip;

    //�� ����
    [SerializeField,Header("�� ����")]
    public LayerMask playerLayermask;

    [SerializeField, Header("���ݹ���")] float ghostSightRange;
    Transform target;


    [SerializeField, Header("�̵��Ÿ�")] 
    float moveDistance = 1f;

    IHittable attackPlayer;


    private bool movingRight = true;
    private Vector2 startPos;


    private void Start()
    {
        startPos = transform.position;
        animatorPlayer = GetComponent<AnimatorPlayer>();
        surprisedAnimator = surprisedAnimator.gameObject.GetComponent<Animator>();
        getSurprisedImote.stop = true;
    }

    private void Update()
    {
        Move();
        DetectPlayer();

        //���� �� ���� ������Ʈ �Ұ��� false���?
        if (getSurprisedImote.stop == false)
        {
            //���� ������Ʈ ����
            surprisedAnimator.gameObject.SetActive(false);
        }
    }

    void Move()
    {
        if (movingRight == true)
        {
            MoveRight();
            HandleRotation();
            if (transform.position.x > startPos.x + moveDistance)
            {
                animatorPlayer.Play(uturnClip, idleClip, true);
                movingRight = false;
            }
        }
        else
        {
            MoveLeft();
            HandleRotation();
            if (transform.position.x < startPos.x - moveDistance)
            {
                animatorPlayer.Play(uturnClip, idleClip, true);
                movingRight = true;
            }
        }
    }

    private void HandleRotation()
    {        
        if (movingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }              
    }

    IEnumerator DoFire(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FireShot();
        }
    }

    void DetectPlayer()
    {
        Collider2D player;
        player = Physics2D.OverlapCircle(transform.position, ghostSightRange, playerLayermask);
        if (player != null)
        {
            if(target == null)
            {
                animatorPlayer.Play(spotteddClip, true);
                animatorPlayer.animator.SetBool("isAttack",true);                
                getSurprisedImote.stop = true;
                attackPlayer = player.GetComponent<IHittable>();
                StartCoroutine(DoFire(1.3f));
            }
            target = player.transform;

            ShowExclamationMark();
            SpottedPlayer();
        }
        else if(target != null)
        {
            animatorPlayer.Play(idleClip, true);
            animatorPlayer.animator.SetBool("isAttack", false);
            attackPlayer = target.GetComponent<IHittable>();
            target = null;
            StopAllCoroutines();
        }
    }
    void FireShot()
    {
        Projectile projectile = GameManager.GetProjectile(womanFire);
        //projectile.Shot(Launcher, attackPlayer, GameManager.ShowEffect, GameManager.Use);
    }
    void SpottedPlayer()
    {        
        MoveStop();
        if (target.position.x <= transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);            
        }
    }    
    void ShowExclamationMark()
    {
        if (surprisedAnimator != null)
        {
            // ����ǥ Ȱ��ȭ
            surprisedAnimator.gameObject.SetActive(true);
        }
    }
    //void HideExclamationMark()
    //{
        //if(surprisedAnimator != null)
        //{
        //    surprisedAnimator.gameObject.SetActive(false);
        //}

        //timer += Time.deltaTime;
        //if (timer > 2)
        //{            
        //    surprisedAnimator.gameObject.SetActive(false);
        //    timer = 0;
        //}
    //}
    
    
}
