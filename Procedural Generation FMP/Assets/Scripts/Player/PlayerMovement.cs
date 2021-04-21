using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rigidBody;
    public Animator animator;

    Vector2 move;

    // Update is called once per frame
    void Update()
    {
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        if(move.sqrMagnitude > 0)
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

    private void FixedUpdate()
    {
        if (move.x != 0 || move.y != 0)
        {
            rigidBody.MovePosition(rigidBody.position + move.normalized * moveSpeed * Time.fixedDeltaTime);
        }

        //rigidBody.velocity = move.normalized * moveSpeed;
    }
}
