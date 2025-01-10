using System.Collections;
using UnityEngine;

/// <summary>
/// 워커를 상속 받은 뛸 수 있는 러너 클래스
/// </summary>
public class Runner : Walker, IJumpable
{
    //점프력
    [SerializeField, Header("점프력"), Range(0, byte.MaxValue)]
    protected float _jumpValue = 5;

    //최대 점프 한도
    [SerializeField, Header("최대 점프 한도"), Range(1, 10)]
    protected byte _jumpLimit = 1;

    //현재 점프 횟수
    [SerializeField]
    private byte _jumpCount = 0;
    /// <summary>
    /// 바닥에 충돌하는지 여부를 확인하는 메서드
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

    //점프를 하게 만드는 메서드
    public virtual void Jump()
    {
        if (_jumpCount > 0 )
        {
            _jumpCount--;
            getRigidbody2D.velocity = new Vector2(0, _jumpValue);
        }
    }

    /// <summary>
    /// 점프를 할 수 있는지 판단하는 메서드
    /// </summary>
    /// <returns>점프가 가능하면 true를 반환함</returns>
    public bool CanJump()
    {
        if(_jumpCount > 0)
        {
            return true;
        }
        return false;
    }
}