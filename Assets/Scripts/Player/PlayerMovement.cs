using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float moveX;
    private bool jumpHeld;

    [SerializeField] private float spriteSize = 1.0f;
    [SerializeField] private Transform spriteTf;

    [SerializeField] private Animator animator;

    public LayerMask ground;
    public Transform rayLeftStart;
    public Transform rayRightStart;
    public float rayLength = 0.4f;
    private bool isGround;
    private bool jumpCooldown;

    public float speed = 5f;
    public float jumpStr = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMove(InputValue value)
    {
        moveX = value.Get<float>();
    }

    void OnJump(InputValue value)
    {
        jumpHeld = value.isPressed;
    }

    void Update()
    {
        bool isGroundLeft = Physics2D.Raycast(rayLeftStart.position, Vector2.down, rayLength, ground);
        bool isGroundRight = Physics2D.Raycast(rayRightStart.position, Vector2.down, rayLength, ground);
        isGround = isGroundLeft || isGroundRight;

        float xVel = moveX * speed;
        float yVel = rb.linearVelocity.y;

        animator.SetFloat("player_speed", Mathf.Abs(xVel));
        animator.SetBool("player_jumping", jumpHeld);
        animator.SetBool("player_isGround", isGround);
        animator.SetFloat("player_yVel", yVel);

        if (isGround){
            animator.SetBool("player_jumping", false);
        }

        if (xVel != 0)
        {
            spriteTf.localScale = new Vector2(xVel > 0 ? spriteSize : -spriteSize, spriteSize);
        }

        if (jumpHeld && isGround && !jumpCooldown)
        {
            yVel = jumpStr;
            jumpCooldown = true;
        }

        if (!jumpHeld)
        {
            jumpCooldown = false;
            
            if (yVel > 0)
            {
                yVel = 0.25f;
            }
        }

        rb.linearVelocity = new Vector2(xVel, yVel);
    }
}
