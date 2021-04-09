using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rigidBody;

    Vector2 move;

    // Update is called once per frame
    void Update()
    {
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + move.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
