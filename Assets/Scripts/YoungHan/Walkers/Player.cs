using System;
using UnityEngine;

/// <summary>
/// ������ �����ϰ� �Ǵ� �÷��̾� Ŭ����
/// </summary>
public sealed class Player : Runner, IHittable
{
    private static readonly Vector2 LeftRotation = new Vector2(0, 180);
    private static readonly Vector2 RightRotation = new Vector2(0, 0);

    public enum Interaction
    {
        Pick,
        MoveUp,
        MoveDown
    }

    [SerializeField]
    private Animator _animator = null;

    public bool isAlive
    {
        get;
    }

    //�ǰ� ���� �׼�

    private Action<Strike, Strike.Area, GameObject> _strikeAction = null;
    
    //������ü �߻� �׼�

    private Func<Interaction, bool> _interactionFunction = null;

    public void Initialize(Action<Strike, Strike.Area, GameObject> strikeAction, Func<Interaction, bool> interactionFunction)
    {
        _strikeAction = strikeAction;
        _interactionFunction = interactionFunction;
    }

    public void Heal()
    {

    }

    public void UseLethalMove()
    {

    }

    public void Attack1()
    {
        //Strike.PolygonArea polygonArea = new Strike.PolygonArea(position, null, null);
        //polygonArea.Show();
        //Strike.TagArea tagArea = new Strike.TagArea(new string[] {"Player"});
        //tagArea.Show();
        Strike.TargetArea targetArea = new Strike.TargetArea(new IHittable[1] { this});
        targetArea.Show();

    }

    public void Attack2()
    {

    }

    public void Attack3()
    {

    }

    public override void MoveLeft()
    {
        base.MoveLeft();
        getTransform.rotation = Quaternion.Euler(LeftRotation);
        _animator?.SetBool("IsWork", true);
    }

    public override void MoveRight()
    {
        base.MoveRight();
        getTransform.rotation = Quaternion.Euler(RightRotation);
        _animator?.SetBool("IsWork", true);
    }

    public override void MoveStop()
    {
        base.MoveStop();
        _animator?.SetBool("IsWork", false);
    }

    public void MoveUp()
    {
        if(_interactionFunction != null && _interactionFunction.Invoke(Interaction.MoveUp) == true)
        {
            //�����ϸ� �ִϸ��̼� �ٲٱ�
        }
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