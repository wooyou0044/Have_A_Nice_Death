using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : Runner, IHittable
{
    int hp;
    Collider2D bossCollider;

    public bool isAlive
    {
        get
        {
            if(hp>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    void Awake()
    {
        bossCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        
    }

    public Collider2D GetCollider2D()
    {
        return bossCollider;
    }

    public void Hit(Strike strike)
    {

    }
}
