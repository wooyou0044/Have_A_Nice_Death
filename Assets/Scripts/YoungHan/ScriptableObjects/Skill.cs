using UnityEngine;

[CreateAssetMenu(menuName = nameof(Skill), order = 0)]
public class Skill : ScriptableObject
{
    public Strike strike;

    public GameObject hit;

    public Projectile[] projectiles;       //발사체들

    public abstract class Target: ScriptableObject
    {
        public abstract Strike.Target GetTarget();
    }

    [CreateAssetMenu(menuName = nameof(PolygonTarget), order = 0)]
    public class PolygonTarget: Target
    {
        [SerializeField]
        private int test;

        public override Strike.Target GetTarget()
        {
            return null;
        }
    }
}
