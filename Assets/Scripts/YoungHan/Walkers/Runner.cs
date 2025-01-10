using System.Collections;
using UnityEngine;

/// <summary>
/// ��Ŀ�� ��� ���� �� �� �ִ� ���� Ŭ����
/// </summary>
public class Runner : Walker, IJumpable
{
    //������
    [SerializeField, Header("������"), Range(0, byte.MaxValue)]
    protected float _jumpValue = 5;

    //�ִ� ���� �ѵ�
    [SerializeField, Header("�ִ� ���� �ѵ�"), Range(1, 10)]
    protected byte _jumpLimit = 1;

    //���� ���� Ƚ��
    [SerializeField]
    private byte _jumpCount = 0;
    /// <summary>
    /// �ٴڿ� �浹�ϴ��� ���θ� Ȯ���ϴ� �޼���
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if(isGrounded == true)
        {
            _jumpCount = _jumpLimit;
        }
    }

    //������ �ϰ� ����� �޼���
    public virtual void Jump()
    {
        if (_jumpCount > 0 )
        {
            _jumpCount--;
            getRigidbody2D.velocity = new Vector2(0, _jumpValue);
        }
    }

    /// <summary>
    /// ������ �� �� �ִ��� �Ǵ��ϴ� �޼���
    /// </summary>
    /// <returns>������ �����ϸ� true�� ��ȯ��</returns>
    public bool CanJump()
    {
        if(_jumpCount > 0)
        {
            return true;
        }
        return false;
    }
}