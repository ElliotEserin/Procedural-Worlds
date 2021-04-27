using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed;
    public Rigidbody2D rigidBody;
    public Animator animator;

    protected Vector2 move;

    protected float speed;

    protected void MoveTowardsTarget()
    {
        if (speed == 0) speed = moveSpeed;

        if (move.x != 0 || move.y != 0)
        {
            rigidBody.MovePosition(rigidBody.position + move.normalized * speed * Time.fixedDeltaTime);
        }
    }

    protected void UpdateAnimator()
    {
        if (move.sqrMagnitude > 0)
        {
            animator.SetBool("Moving", true);

            animator.SetFloat("Horizontal", move.x);
            animator.SetFloat("Vertical", move.y);
        }

        else
        {
            animator.SetBool("Moving", false);
        }
    }
}
