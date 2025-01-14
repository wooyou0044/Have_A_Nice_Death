using System.Collections;
using UnityEngine;

/// <summary>
/// ��Ŀ�� ��� ���� �� �� �ִ� ���� Ŭ����
/// </summary>
public class Runner : Walker
{
    //���� �� �ٽ� ���� �� �� �ְ� ��ٸ��� �ּ� �ð�
    private static readonly float JumpDelay = 0.05f;

    //������
    [SerializeField, Header("���� ����"), Range(0, byte.MaxValue)]
    protected float _jumpValue = 5;

    //�ִ� ���� �ѵ�
    [SerializeField, Header("�ִ� ���� �ѵ�"), Range(1, 10)]
    protected byte _jumpLimit = 1;

    //���� ���� Ƚ��
    [SerializeField]
    private byte _jumpCount = 0;

    private IEnumerator _jumpCoroutine = null;

    [SerializeField, Header("�뽬 ����"), Range(0, 100)]
    protected float _dashValue = 50;
    [SerializeField, Header("�뽬 ���� �ð�"), Range(0, 5)]
    private float _dashDelay = 0.2f;
    [SerializeField, Header("�뽬 ��Ÿ��"), Range(0, 5)]
    protected float _dashCoolTime = 1.5f;

    [SerializeField]
    private bool _isDashed = false;

    private IEnumerator _dashCoroutine = null;

    [SerializeField, Header("���ߺξ� �ð�"), Range(0.1f, 10)]
    protected float _levitationDelay = 0.1f;

    [SerializeField, Header("���ߺξ� ��Ÿ��"), Range(0, 5)]
    protected float _levitationCoolTime = 0.3f;

    private IEnumerator _levitationCoroutine = null;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (isGrounded == true)
        {
            _jumpCount = _jumpLimit;
        }
        else
        {
            _jumpCount = 0;
        }
    }
#endif

    /// <summary>
    /// �ٴڿ� �浹�ϴ��� ���θ� Ȯ���ϴ� �޼���
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
            _jumpCount = _jumpLimit;
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

    //������ �ϰ� ����� �޼���
    public virtual void Jump()
    {
        if (_jumpCount > 0 && _jumpCoroutine == null && getRigidbody2D.gravityScale > 0)
        {
            StopLevitate();
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

    //�뽬�� �ϰ� ����� �޼���
    public virtual void Dash()
    {
        if (_dashCoroutine == null && _isDashed == false)
        {
            StopLevitate();
            _dashCoroutine = DoDashAndDelay();
            StartCoroutine(_dashCoroutine);
            IEnumerator DoDashAndDelay()
            {
                _isDashed = true;
                getRigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                getRigidbody2D.velocity = new Vector2(getTransform.forward.normalized.z * _dashValue, 0);
                yield return new WaitForSeconds(_dashDelay);
                getRigidbody2D.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
                getRigidbody2D.velocity += Vector2.down;
                _dashCoroutine = null;
                yield return new WaitForSeconds(_dashCoolTime);
                _isDashed = false;
            }
        }
    }

    private void StopLevitate()
    {
        if (_levitationCoroutine != null)
        {
            StopCoroutine(_levitationCoroutine);
            _levitationCoroutine = null;
            getRigidbody2D.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    protected void Levitate(bool force)
    {
        Levitate(force, _levitationDelay);
    }

    protected void Levitate(bool force, float duration)
    {
        if(force == true)
        {
            if(_levitationCoroutine != null)
            {
                StopCoroutine(_levitationCoroutine);
            }
            _levitationCoroutine = DoLevitateAndDelay();
            StartCoroutine(_levitationCoroutine);
        }
        else if(_levitationCoroutine == null)
        {
            _levitationCoroutine = DoLevitateAndDelay();
            StartCoroutine(_levitationCoroutine);
        }
        IEnumerator DoLevitateAndDelay()
        {
            getRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            yield return new WaitForSeconds(duration);
            getRigidbody2D.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            getRigidbody2D.velocity += Vector2.down;
            yield return new WaitForSeconds(_levitationCoolTime);
            _levitationCoroutine = null;
        }
    }

    public bool IsLevitate()
    {
        return _levitationCoroutine != null;
    }
}