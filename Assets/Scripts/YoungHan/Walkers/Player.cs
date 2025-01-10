using System;
using UnityEngine;

/// <summary>
/// 유저가 조종하게 되는 플레이어 클래스
/// </summary>
[RequireComponent(typeof(Animator))]
public sealed class Player : Runner, IHittable
{
    private bool _hasAnimator = false;

    private Animator _animator = null;

    private Animator getAnimator
    {
        get
        {
            if (_hasAnimator == false)
            {
                _hasAnimator = true;
                _animator = GetComponent<Animator>();
            }
            return _animator;
        }
    }

    public bool isAlive
    {
        get
        {
            return true;
        }
    }

    //피격 액션 델리게이트
    private Action<IHittable, int> _hitAction = null;
    //스킬 사용 액션 델리게이트
    
    //private Func<Interaction, bool> _interactionFunction = null;

    public void Initialize(Action<IHittable, int> hit)
    {
        _hitAction = hit;
        //_strikeAction = strike;
        //_interactionFunction = interaction;
    }

    public void Heal()
    {

    }

    public void UseLethalMove()
    {

    }

    public void Attack1()
    {

    }

    public void Attack2()
    {

    }

    public void Attack3()
    {

    }

    public override void MoveLeft()
    {
        if (isAlive == true)
        {
            base.MoveLeft();
            getTransform.rotation = Quaternion.Euler(LeftRotation);
            getAnimator.SetBool("Run", true);
        }
    }

    public override void MoveRight()
    {
        if (isAlive == true)
        {
            base.MoveRight();
            getTransform.rotation = Quaternion.Euler(RightRotation);
            getAnimator.SetBool("Run", true);
        }
    }

    public override void MoveStop()
    {
        if (isAlive == true)
        {
            base.MoveStop();
            getAnimator.SetBool("Run", false);
        }
    }

    public void MoveUp()
    {
        //if(_interactionFunction != null && _interactionFunction.Invoke(Interaction.MoveUp) == true)
        //{
        //    //성공하면 애니메이션 바꾸기
        //}
    }

    public void MoveDown()
    {

    }

    public void Hit(Strike strike)
    {

    }

    public Collider2D GetCollider2D()
    {
        return getCollider2D;
    }
}