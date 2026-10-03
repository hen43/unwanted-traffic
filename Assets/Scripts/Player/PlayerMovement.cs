using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveX { get; private set;}
    public float moveY { get; private set;}
    public int dir { get; private set; }
    private bool jumpHeld;
    private bool jumpPressedThisFrame;
    
    [SerializeField] private float grabRadius;

    [SerializeField] private float spriteSize = 1.0f;
    [SerializeField] private Transform spriteTf;

    [SerializeField] private Animator animator;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private ShopHandler shopHandler;

    [SerializeField] private CurrencyHandler currencyHandler;

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
    private bool yVelOverride;
    private bool launching;
    [SerializeField] private float launchXVel = 20f;
    private float launchDuration;

    private bool isJumping;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                playerStats = GetComponentInChildren<PlayerStats>();
            }
        }
    }

    void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveX = input.x;
        moveY = input.y;
        // Debug.Log($"x: {moveX}, y: {moveY}");
    }

    void OnJump(InputValue value)
    {
        jumpHeld = value.isPressed;

        if (value.isPressed)
        {
            jumpPressedThisFrame = true;
        }
    }

    public void SetYVel(float value)
    {
        yVel = value;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, yVel);
        yVelOverride = true;
        isJumping = false;
    }

    public void Launch(float amt)
    {
        launchDuration = 0.125f;
        launchXVel = amt;
        launching = true;
    }

    // unused
    // public float FacingDir()
    // {
    //     return spriteTf.localScale.x > 0 ? 1f : -1f;
    // }

    void VoidNet()
    {
        if (transform.position.y < -30)
        {
            if(SceneManager.GetActiveScene().name != "Tutorial")
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                
                xVel = 0f;
                yVel = 0f;

                MapHandler mapHandler = Object.FindAnyObjectByType<MapHandler>();
                if (mapHandler != null)
                {
                    mapHandler.ResetMap();
                }
            } else {
                SceneManager.LoadScene("Main Game");
            }
        }
    }

    void Start()
    {
        launching = false;
        launchDuration = 0;

        if (currencyHandler == null)
        {
            currencyHandler = GetComponentInParent<CurrencyHandler>();
            if (currencyHandler == null)
            {
                currencyHandler = FindAnyObjectByType<CurrencyHandler>();
            }
        }

        if (playerStats == null)
        {
            playerStats = GetComponentInParent<PlayerStats>();
            if (playerStats == null)
            {
                playerStats = FindAnyObjectByType<PlayerStats>();
            }
        }

        if (shopHandler == null)
        {
            shopHandler = GetComponentInParent<ShopHandler>();
            if (shopHandler == null)
            {
                shopHandler = FindAnyObjectByType<ShopHandler>();
            }
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Play) 
            return;

        VoidNet();

        if(launchDuration > 0f)
        {
            launchDuration -= Time.deltaTime;
        }
        else
        {
            launching = false;
        }

        bool revenge = (GameManager.Instance != null && GameManager.Instance.CurrentPlayerState == GameManager.PlayerState.Revenge);
        
        float revengeBoost = 1f;
        float currentJump = jumpStr;

        if (revenge && playerStats != null && currencyHandler != null)
        {
            RevengeStats stats = playerStats.CalculateRevenge(currencyHandler.StartingRevengeBlood);
            revengeBoost = stats.speed;
            currentJump *= stats.jump;
        }

        bool isGroundLeft = Physics2D.Raycast(rayLeftStart.position, Vector2.down, rayLength, ground | prop);
        bool isGroundRight = Physics2D.Raycast(rayRightStart.position, Vector2.down, rayLength, ground | prop);
        isGround = isGroundLeft || isGroundRight;

        bool isGrabbable = Physics2D.OverlapCircle(grab.position, grabRadius, ground | prop) && !isGround;

        float finalSpeed = 0f;
        if (playerStats != null)
        {
            finalSpeed = playerStats.getDefaultStat(PlayerStats.Stat.Speed);
        }

        ShopHandler shopHandler = Object.FindAnyObjectByType<ShopHandler>();
        if (shopHandler != null)
        {
            finalSpeed += (1 * shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Speed));
        }

        xVel = moveX * finalSpeed * revengeBoost;

        if(!yVelOverride)
        {
            yVel = rb.linearVelocity.y;
        }

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
            if(moveX > 0)
            {
                spriteTf.localScale = new Vector2(spriteSize, spriteSize);
                dir = 1;
            } 
            else 
            {
                spriteTf.localScale = new Vector2(-spriteSize, spriteSize);
                dir = -1;
            }
        }

        if (jumpPressedThisFrame && isGrabbable)
        {
            yVel = launchStr;
            coyoteTime = 0f;
            jumpCooldown = true;
            isJumping = true;
        }
        else if (jumpHeld && (isGround || coyoteTime > 0) && !jumpCooldown)
        {
            yVel = currentJump;
            jumpCooldown = true;
            coyoteTime = 0f;
            isJumping = true;
        }

        if (!jumpHeld && isJumping)
        {
            jumpCooldown = false;
            isJumping = false;

            if (yVel > 0)
            {
                yVel = 0.25f;
            }
        }
        else if (!jumpHeld)
        {
            jumpCooldown = false;
        }

        if(!launching)
        {
            rb.linearVelocity = new Vector2(xVel, yVel);
        }
        else
        {
            rb.linearVelocity = new Vector2(launchXVel, yVel);
        }

        jumpPressedThisFrame = false;
        yVelOverride = false;
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