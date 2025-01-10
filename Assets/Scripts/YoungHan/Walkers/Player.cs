using System;
using UnityEngine;

/// <summary>
/// ������ �����ϰ� �Ǵ� �÷��̾� Ŭ����
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

    //�ǰ� �׼� ��������Ʈ
    private Action<IHittable, int> _hitAction = null;
    //��ų ��� �׼� ��������Ʈ
    
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
        //    //�����ϸ� �ִϸ��̼� �ٲٱ�
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