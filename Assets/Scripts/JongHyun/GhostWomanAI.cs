using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWomanAI : MonoBehaviour
{
    
    public Animator animator;
    public float moveSpeed = 0.01f;
    public float moveDistance = 1f;
    private Vector2 startPos;
    private bool movingRight = true;

    void Start()
    {
        startPos = transform.position;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        Move();       
        WomanAnimation();
    }
    void Move()
    {
        if (movingRight)
        {
            HandleRotation();
            transform.position += new Vector3(moveSpeed*Time.deltaTime, 0, 0);
            if (transform.position.x > startPos.x + moveDistance)
            {
                movingRight = false;
            }
        }
        else
        {
            HandleRotation();
            transform.position -= new Vector3(moveSpeed * Time.deltaTime, 0, 0);
            if (transform.position.x < startPos.x - moveDistance)
            {
                movingRight = true;
            }
        }
    }
    private void WomanAnimation()
    {
        Vector2 movement = movingRight ? Vector2.right : Vector2.left;
        if (animator != null)
        {
            animator.SetBool("IsWalk", true);
        }
    }
    private void HandleRotation()
    {
        if (movingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);            
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);            
        }
    }
    
}