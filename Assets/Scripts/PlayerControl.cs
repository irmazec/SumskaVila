using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 12.5f;
    public float jumpSpeed = 15;
    public float maxSprintFactor = 3;
    public Vector2 mapStartCorner;
    public Vector2 mapEndCorner;
    public int mapTop;

    public GameObject infoScreen;
    public GameObject pauseMenu;       
    private bool isPaused = false;

    private float defaultMoveSpeed;
    private int moveFwd = 0;
    private int moveRight = 0;
    private int moveUp = 0;
    private bool usingSprint = false;
    private float sprintStarted = 0;
    private bool useJump = false;
    private bool isFlying = false;
    private float jumpInputTimestamp = 0;
    private int isOnFloor = 0;
    private bool isInDialogue = false;
    private float[] borderDistances = new float[] { };
    private Rigidbody rb;
    private MouseLook mouseLookPlayer;
    private MouseLook mouseLookCamera;

    void Start()
    {
        defaultMoveSpeed = moveSpeed;
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mouseLookPlayer = GetComponent<MouseLook>();
        mouseLookCamera = Camera.main.GetComponent<MouseLook>();

        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (GameManager.GM.gameEndScreenActive)
        {
            if (Time.timeScale != 0)
            {
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            return;
        }

        var keyboard = Keyboard.current;

        // Pause menu toggle
        if ((keyboard[Key.P].wasPressedThisFrame && !isInDialogue) || (keyboard.escapeKey.wasPressedThisFrame && !isInDialogue))
        {
            TogglePause();
        }

        if (isPaused)
            return;

        if (Time.time - jumpInputTimestamp > 0.1f)
            useJump = false;

        // Basic inputs (left, right, up, sprint)
        moveFwd = 0;
        moveRight = 0;
        moveUp = 0;
        usingSprint = false;

        if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed)
            moveFwd += 1;
        if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed)
            moveFwd -= 1;
        if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
            moveRight += 1;
        if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
            moveRight -= 1;
        if (keyboard.leftCtrlKey.isPressed)
        {
            usingSprint = true;
            if (sprintStarted == 0) sprintStarted = Time.time;
        }
        else sprintStarted = 0;

        // Jump and fly toggle
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            if (Time.time - jumpInputTimestamp <= 0.25f)
            {
                isFlying = !isFlying;
                rb.useGravity = !rb.useGravity;
            }
            else
            {
                useJump = true;
                jumpInputTimestamp = Time.time;
            }
        }
        // Flying inputs (ascend, descend), fly exit on ground
        if (isFlying)
        {
            if (keyboard.spaceKey.isPressed)
                moveUp += 1;
            if (keyboard.leftShiftKey.isPressed)
                moveUp -= 1;
            
            if (isOnFloor >= 1)
            {
                isFlying = false;
                rb.useGravity = true;
            }
        }

        // Info screen toggle
        if (keyboard.tabKey.wasPressedThisFrame && !isInDialogue)
        {
            SetInfoScreen(!infoScreen.activeInHierarchy);
        }

        // Border logic
        borderDistances = new float[] {
                    Mathf.Abs(transform.position.x - mapStartCorner.x),
                    Mathf.Abs(transform.position.x - mapEndCorner.x),
                    Mathf.Abs(transform.position.z - mapStartCorner.y),
                    Mathf.Abs(transform.position.z - mapEndCorner.y),
                    Mathf.Abs(transform.position.y - mapTop)
        };
        moveSpeed = defaultMoveSpeed * (1 - (10 - Mathf.Min(Mathf.Min(borderDistances), 10)) * 0.08f);
    }

    void FixedUpdate()
    {
        if (isInDialogue)
            return;

        // Horizontal movement direction
        Vector3 moveDir = (moveFwd * new Vector3(transform.forward.x, 0, transform.forward.z) + moveRight * transform.right).normalized;
        moveDir *= usingSprint ? Mathf.Min(4, Mathf.Max(1.5f, 1.5f + (int)(Time.time - sprintStarted - 10) * 0.05f)) : 1;
        
        // Vertical velocity setup (gravity, flying mode, jump)
        float verticalVelocity = rb.velocity.y;
        if (isFlying)
            verticalVelocity = Mathf.Lerp(rb.velocity.y, moveUp * moveSpeed, Time.deltaTime * 8);
        else if (useJump && isOnFloor >= 1)
        {
            verticalVelocity = jumpSpeed;
            useJump = false;
        }

        // Applying movement to velocity
        rb.velocity = new Vector3(
            Mathf.Lerp(rb.velocity.x, moveDir.x * moveSpeed, Time.deltaTime * 6),
            verticalVelocity,
            Mathf.Lerp(rb.velocity.z, moveDir.z * moveSpeed, Time.deltaTime * 6)
        );
    }

    // Helper function to be called by floor detector trigger
    public void setIsOnFloor(int dir)
    {
        isOnFloor += dir;
    }

    void SetInfoScreen(bool enabled)
    {
        infoScreen.SetActive(enabled);
        SetCamera(!enabled);
        Cursor.visible = enabled;
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void SetCamera(bool enabled)
    {
        if (mouseLookPlayer != null)
            mouseLookPlayer.enabled = enabled;

        if (mouseLookCamera != null)
            mouseLookCamera.enabled = enabled;
    }

    public void ToggleDialogue()
    {
        SetInfoScreen(false);
        SetCamera(isInDialogue);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = isInDialogue ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeAll;
        isInDialogue = !isInDialogue;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);  // display pause menu

        SetCamera(!isPaused);   // disable camera when paused

        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        Time.timeScale = isPaused ? 0f : 1f;    // pause time
    }

    public void QuitToTitle()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
