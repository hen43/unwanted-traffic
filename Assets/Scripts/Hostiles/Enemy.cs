using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, AnimationReceiver
{
    private Rigidbody2D rb;
    public Animator animator;
    private CameraMovement cam;

    [Header("Scriptable Object Data")]
    [SerializeField] private EnemyData enemyData;

    [SerializeField] private SpriteRenderer rend;
    [SerializeField] private Transform spriteTransform;

    private CurrencyHandler currencyHandler; 
    private Color ogColor;
    public GameObject deathParticle;

    private float flashDuration = 0.05f; 

    private int maxHealth;
    // private int damage;
    private int damage = 10; // for testing
    private int speed;
    private int currentHealth;

    [SerializeField] private float detectionRange = 12f;
 
    private bool currentFlip;
    private int dir;

    [SerializeField] private Transform wallCheckOrigin;
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackRangeHeight;
    public LayerMask wallCheckLayer;
    public LayerMask playerLayer;
    public LayerMask ground;
    private float groundCheckLength = 0.8f;  

    void Start()
    {
        cam = CameraMovement.instance;
        rb = GetComponent<Rigidbody2D>();

        if (enemyData != null)
        {
            maxHealth = enemyData.health;
            damage = enemyData.damage;
            speed = enemyData.speed + Random.Range(-3,4);
        }

        if (spriteTransform == null)
        {
            SpriteRenderer childRend = GetComponentInChildren<SpriteRenderer>();
            spriteTransform = childRend != null ? childRend.transform : transform;
        }

        if (rend == null)
        {
            rend = GetComponentInChildren<SpriteRenderer>();
        }

        if (currencyHandler == null)
        {
            currencyHandler = FindAnyObjectByType<CurrencyHandler>();
        }

        if (rend != null)
        {
            ogColor = rend.color;
        }

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (Player.Instance == null)
        {
            dir = 0;
            return;
        }

        if (rend == null) rend = GetComponentInChildren<SpriteRenderer>();

        float playerX = Player.Instance.transform.position.x;
        float currentX = transform.position.x;
        float playerY = Player.Instance.transform.position.y;
        float currentY = transform.position.y;
        float distanceToPlayer = Mathf.Abs(playerX - currentX);
        float heightToPlayer = Mathf.Abs(playerY - currentY);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > 0.1f)
            {
                currentFlip = playerX < currentX;
                
                Vector3 localScale = transform.localScale;
                localScale.x = currentFlip ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
                transform.localScale = localScale;

                if (rend != null) rend.flipX = false; 
            }

            dir = (playerX > currentX) ? 1 : -1;

            bool playerInRange = (distanceToPlayer < attackRange) && (heightToPlayer < attackRangeHeight);
            if (animator != null) animator.SetBool("PlayerInRange", playerInRange);

            if(!playerInRange)
            {
                JumpCheck();
            } else {
                dir = 0;
            }

        }
        else
        {
            dir = 0;
        }
    }

    public void OnAnimationEvent(string eventName)
    {
        if (eventName == "Attack")
        {
            Collider2D hitPlayer = Physics2D.OverlapCircle(attackOrigin.position, attackRadius, playerLayer);
            if (hitPlayer != null)
            {
                currencyHandler?.ChangeBlood(damage * -1);
                cam.Shake(0.2f);
            }
        }
    }

    private void JumpCheck()
    {
        bool isGrounded = Physics2D.Raycast(wallCheckOrigin.position, Vector2.down, groundCheckLength, wallCheckLayer | ground);

        if (dir != 0)
        {
            float wallCheckLength = speed / 2f;
            Vector2 origin = (wallCheckOrigin != null) ? (Vector2)wallCheckOrigin.position : (Vector2)transform.position;
            Vector2 wallCheckDir = new Vector2(dir, 0);

            RaycastHit2D wallCheck = Physics2D.Raycast(origin, wallCheckDir, wallCheckLength, wallCheckLayer);
            Debug.DrawRay(origin, wallCheckDir * wallCheckLength, Color.red);

            if (wallCheck.collider != null)
            {                
                Vector2 wallHitPosition = wallCheck.point;
                float heightCheckHeight = 20f;
                Vector2 heightCheckOrigin = wallHitPosition + new Vector2(dir * 0.1f, heightCheckHeight);
                RaycastHit2D heightCheck = Physics2D.Raycast(heightCheckOrigin, Vector2.down, heightCheckHeight + 5f, wallCheckLayer);

                if (heightCheck.collider != null)
                {
                    float obstacleHeight = heightCheck.point.y - transform.position.y;

                    if (isGrounded && Mathf.Abs(obstacleHeight) > 0f)
                    {
                        ExecuteJump(obstacleHeight);
                    }
                }

                Debug.DrawRay(heightCheckOrigin, Vector2.down * (heightCheckHeight + 5f), Color.red);
            }
        }
    }

    private void ExecuteJump(float obstacleHeight)
    {
        float clearanceBuffer = 4f + Random.Range(-0.5f, 0.5f);
        float targetHeight = obstacleHeight + clearanceBuffer;
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float jumpVelocity = Mathf.Sqrt(2f * gravity * targetHeight);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector2(speed * dir, rb.linearVelocity.y);
    }

    private IEnumerator DeathSequence(float shakeX, float shakeY)
    {
        yield return StartCoroutine(dmgIndicator());
        Die(shakeX, shakeY);
    }

    private IEnumerator dmgIndicator()
    {
        if (rend != null) rend.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        if (rend != null) rend.color = ogColor;
    } 

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            float cash = enemyData.cashDrop;
            int bloodAmt = Mathf.FloorToInt(cash + Random.Range(-(cash*0.25f), (cash*0.25f)));
            currencyHandler?.ChangeBlood(bloodAmt);
            
            StartCoroutine(DeathSequence(0.5f, 0f));
        }
        else
        {
            StartCoroutine(dmgIndicator());
        }
    }

    public void EnvironmentDeath(float shakeX = 0f, float shakeY = 0f)
    {
        StartCoroutine(DeathSequence(shakeX, shakeY));
    }

    void Die(float shakeX, float shakeY)
    {
        if (cam != null && (shakeX > 0f || shakeY > 0f))
        {
            cam.Shake(shakeX, shakeY);
        }

        GameObject spawnedParticle = Instantiate(deathParticle, transform.position + new Vector3(0, -1, 0), deathParticle.transform.rotation);
        Destroy(gameObject, 0.0f);
        Destroy(spawnedParticle, 1.0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackOrigin.position, attackRadius); 
        }
    }
}