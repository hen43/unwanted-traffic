using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float moveX;
    private bool jumpHeld;
    private bool jumpPressedThisFrame;
    
    [SerializeField] private float grabRadius;

    [SerializeField] private float spriteSize = 1.0f;
    [SerializeField] private Transform spriteTf;

    [SerializeField] private Animator animator;

    public LayerMask ground;
    public LayerMask prop;
    public Transform grab;
    public Transform rayLeftStart;
    public Transform rayRightStart;
    public float rayLength = 0.4f;
    private bool isGround;
    private bool jumpCooldown;

    public float speed = 5f;
    public float jumpStr = 10f;
    public float launchStr = 14f;
    private float coyoteTime = 0f;

    private float xVel;
    private float yVel;

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

        if (value.isPressed)
        {
            jumpPressedThisFrame = true;
        }
    }

    public float FacingDir()
    {
        return spriteTf.localScale.x > 0 ? 1f : -1f;
    }

    void VoidNet()
    {
        if (transform.position.y < -30)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            
            xVel = 0f;
            yVel = 0f;

            transform.position = new Vector3(0, 0, 0);
        }
    }

    void Update()
    {
        VoidNet();

        bool isGroundLeft = Physics2D.Raycast(rayLeftStart.position, Vector2.down, rayLength, ground | prop);
        bool isGroundRight = Physics2D.Raycast(rayRightStart.position, Vector2.down, rayLength, ground | prop);
        isGround = isGroundLeft || isGroundRight;

        bool isGrabbable = Physics2D.OverlapCircle(grab.position, grabRadius, ground | prop) && !isGround;

        xVel = moveX * speed;
        yVel = rb.linearVelocity.y;

        animator.SetFloat("speed", Mathf.Abs(xVel));
        animator.SetBool("jumping", jumpHeld);
        animator.SetBool("isGround", isGround);
        animator.SetFloat("yVel", yVel);

        if (coyoteTime > 0 && !isGround)
        {
            coyoteTime -= Time.deltaTime;
        }

        if (isGround)
        {
            animator.SetBool("jumping", false);
            coyoteTime = 0.33f;
        }

        if (moveX != 0)
        {
            spriteTf.localScale = new Vector2(moveX > 0 ? spriteSize : -spriteSize, spriteSize);
        }

        if (jumpPressedThisFrame && isGrabbable)
        {
            yVel = launchStr;
            coyoteTime = 0f;
            jumpCooldown = true;
        }
        else if (jumpHeld && (isGround || coyoteTime > 0) && !jumpCooldown)
        {
            yVel = jumpStr;
            jumpCooldown = true;
            coyoteTime = 0f;
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

        jumpPressedThisFrame = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (grab != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(grab.position, grabRadius);
        }
    }
}