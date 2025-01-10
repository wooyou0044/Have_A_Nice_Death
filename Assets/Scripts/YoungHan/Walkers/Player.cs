using System;
using UnityEngine;

/// <summary>
/// ������ �����ϴ� �÷��̾� Ŭ����
/// </summary>
[RequireComponent(typeof(Animator))]
public sealed class Player : Runner, IHittable
{
    private static readonly string MoveAnimation = "Move";
    private static readonly string JumpAnimation = "Jump";

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

    //�ǰ� �׼� ��������Ʈ
    private Action<IHittable, int> _hitAction = null;
    //��ų ��� �׼� ��������Ʈ

    //private Func<Interaction, bool> _interactionFunction = null;

    public void Initialize(Action<IHittable, int> hit)
    {
        _hitAction = hit;
        //_strikeAction = strike;        //_interactionFunction = interaction;
    } 

    public override void MoveLeft()
    {
        if (isAlive == true)
        {
            base.MoveLeft();
            getTransform.rotation = Quaternion.Euler(LeftRotation);
            getAnimator.SetBool(MoveAnimation, true);
        }
    }

    public override void MoveRight()
    {
        if (isAlive == true)
        {
            base.MoveRight();
            getTransform.rotation = Quaternion.Euler(RightRotation);
            getAnimator.SetBool(MoveAnimation, true);
        }
    }

    public override void MoveStop()
    {
        if (isAlive == true)
        {
            base.MoveStop();
            getAnimator.SetBool(MoveAnimation, false);
        }
    }

    public void MoveUp()
    {
        //if(_interactionFunction != null && _interactionFunction.Invoke(Interaction.MoveUp) == true)
        //{
        //    //�����ϸ� �ִϸ��̼� �ٲٱ�
        //}
    }

    public void MoveDown()
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

    public void AttackWide()
    {

    }

    public void Interact()
    {

    }

    public void Heal()
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