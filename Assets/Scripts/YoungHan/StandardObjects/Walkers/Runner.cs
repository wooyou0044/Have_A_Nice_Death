using System.Collections;
using UnityEngine;

/// <summary>
/// 워커를 상속 받은 뛸 수 있는 러너 클래스
/// </summary>
public class Runner : Walker
{
    //점프 후 다시 점프 할 수 있게 기다리는 최소 시간
    private static readonly float JumpDelay = 0.05f;

    //점프력
    [SerializeField, Header("점프 강도"), Range(0, byte.MaxValue)]
    protected float _jumpValue = 5;

    //최대 점프 한도
    [SerializeField, Header("최대 점프 한도"), Range(1, 10)]
    protected byte _jumpLimit = 1;

    //현재 점프 횟수
    [SerializeField]
    private byte _jumpCount = 0;

    private IEnumerator _jumpCoroutine = null;

    [SerializeField, Header("대쉬 강도"), Range(0, 100)]
    protected float _dashValue = 50;
    [SerializeField, Header("대쉬 지속 시간"), Range(0, 5)]
    private float _dashDelay = 0.2f;
    [SerializeField, Header("대쉬 쿨타임"), Range(0, 5)]
    protected float _dashCoolTime = 1.5f;

    [SerializeField]
    private bool _isDashed = false;

    private IEnumerator _dashCoroutine = null;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (isGrounded == true)
        {
            RecoverJumpCount();
        }
        else
        {
            _jumpCount = 0;
        }
    }
#endif

    /// <summary>
    /// 바닥에 충돌하는지 여부를 확인하는 메서드
    /// </summary>
    /// <param name="collision"></param>
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if(isGrounded == true)
        {
            if(_jumpCoroutine != null)
            {
                StopCoroutine(_jumpCoroutine);
                _jumpCoroutine = null;
            }
            RecoverJumpCount();
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        bool isGrounded = this.isGrounded;
        base.OnCollisionStay2D(collision);
        if(isGrounded != this.isGrounded)
        {
            RecoverJumpCount();
        }
    }

    public override void MoveLeft()
    {
        if (_dashCoroutine == null)
        {
            base.MoveLeft();
        }
    }

    public override void MoveRight()
    {
        if (_dashCoroutine == null)
        {
            base.MoveRight();
        }
    }

    public override void MoveStop()
    {
        if (_dashCoroutine == null)
        {
            base.MoveStop();
        }
    }

    //점프를 하게 만드는 메서드
    public virtual void Jump()
    {
        if (_jumpCount > 0 && _jumpCoroutine == null && getRigidbody2D.gravityScale > 0)
        {
            //StopLevitate();
            _jumpCoroutine = DoJumpAndDelay();
            StartCoroutine(_jumpCoroutine);
            IEnumerator DoJumpAndDelay()
            {
                _jumpCount--;
                getRigidbody2D.velocity = new Vector2(0, _jumpValue);
                yield return new WaitForSeconds(_jumpValue / getRigidbody2D.gravityScale * JumpDelay);
                _jumpCoroutine = null;
            }
        }
    }

    public virtual void Dash(Vector2 direction)
    {
        Dash(direction, _dashDelay);
    }

    protected void RecoverJumpCount()
    {
        _jumpCount = _jumpLimit;
    }

    public void Levitate(float value)
    {
        Dash(Vector2.zero, value);
    }

    //대쉬를 하게 만드는 메서드
    public void Dash(Vector2 direction, float value)
    {
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
        }
        _dashCoroutine = DoDashAndDelay();
        StartCoroutine(_dashCoroutine);
        IEnumerator DoDashAndDelay()
        {
            _isDashed = true;
            if(direction == Vector2.zero)
            {
                getRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else if(direction == Vector2.up || direction == Vector2.down)
            {
                getRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
            else if (direction == Vector2.left || direction == Vector2.right)
            {
                getRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
            getRigidbody2D.velocity = direction * _dashValue;
            yield return new WaitForSeconds(value);
            getRigidbody2D.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            getRigidbody2D.velocity += Vector2.down;
            _dashCoroutine = null;
            yield return new WaitForSeconds(_dashCoolTime);
            _isDashed = false;
        }
    }

    public void DashLeft()
    {
        Dash(new Vector2(-1, 0));
    }

    public void DashRight()
    {
        Dash(new Vector2(+1, 0));
    }

    public bool CanDash()
    {
        if(_dashCoroutine == null && _isDashed == false)
        {
            return true;
        }
        return false;
    }
}