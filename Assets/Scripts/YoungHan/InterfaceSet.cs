using UnityEngine;



/// <summary>
/// 특정 위치에서 소환해야 되는 상황에 쓰는 인터페이스
/// </summary>
public interface IPositionable
{
    //위치 지정이 가능한 프로퍼티
    Vector2 position {
        get;
        set;
    }
}

/// <summary>
/// 오른쪽 또는 왼쪽으로 이동하거나 바라볼 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface IMovable
{
    //다른 물체와 충돌 시 유효 거리
    protected static readonly float OverlappingDistance = 0.02f;
    //땅에 착지 중 땅을 이탈했다고 간주하는 최소 속도
    protected static readonly float MinimumDropVelocity = -0.49f;
    //오른쪽으로 이동
    public void MoveRight();
    //왼쪽으로 이동
    public void MoveLeft();
    //이동 멈춤
    public void MoveStop();
}

/// <summary>
/// 뛰어 오를 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface IJumpable
{
    //뛰어 오르기
    public void Jump();
    //뛸 수 있는지 반환
    public bool CanJump();
}

/// <summary>
/// 피격 당할 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface IHittable
{
    public bool isAlive {
        get;
    }

    public string tag
    {
        set;
        get;
    }

    public void Hit(Strike strike);

    //public void Hit(Spell[] spells);

    public Collider2D GetCollider2D();
}

/// <summary>
/// 상호 작용 할 수 있는 객체들이 상속 받는 인터페이스
/// </summary>
public interface Iinteractable
{
    public bool CanInteraction(Vector2 position);
}