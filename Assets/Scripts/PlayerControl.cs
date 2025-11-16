using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 12.5f;
    public float jumpSpeed = 15;

    private int moveFwd = 0;
    private int moveRight = 0;
    private bool useJump = false;
    private float jumpInputTimestamp = 0;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - jumpInputTimestamp > 0.1f)
            useJump = false;

        var keyboard = Keyboard.current;

        // Basic inputs (left, right, up)
        moveFwd = 0;
        moveRight = 0;
        if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed)
            moveFwd += 1;
        if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed)
            moveFwd -= 1;
        if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
            moveRight += 1;
        if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
            moveRight -= 1;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            useJump = true;
            jumpInputTimestamp = Time.time;
        }
    }

    void FixedUpdate()
    {
        Vector3 moveDir = (moveFwd * new Vector3(transform.forward.x, 0, transform.forward.z) + moveRight * transform.right).normalized;
        rb.velocity = new Vector3(moveDir.x * moveSpeed, useJump ? jumpSpeed : rb.velocity.y, moveDir.z * moveSpeed);
        if (useJump)
            useJump = false;
    }
}
