using UnityEngine;

public partial class BossMovement : Runner, IHittable
{
    [SerializeField]
    private Projectile _dashProjectile;

    public void AdjustRotation()
    {
        Vector2 velocity = getRigidbody2D.velocity;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        if (velocity.x >= 0)
        {
            getTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            getTransform.rotation = Quaternion.Euler(180, 0, -angle);
        }
    }

    public void UseDashSkill(float duration)
    {
        Projectile projectile = GameManager.GetProjectile(_dashProjectile);
        projectile.Shot(getTransform, null, GameManager.ShowEffect, GameManager.Use, null, duration);
    }
}
