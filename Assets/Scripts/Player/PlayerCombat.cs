using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Animator animator;
    private PlayerMovement playerMovement;
    private CameraMovement cam;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CurrencyHandler currencyHandler;

    private int baseAttackDmg = 35;
    public Vector2 attackBox;
    private float attackCooldown = 0f;
    public SpriteRenderer attackEffect;

    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        cam = CameraMovement.instance;

        if (attackEffect != null)
        {
            attackEffect.enabled = false;
        }

        // Auto-assign dependencies dynamically if spawned at runtime
        if (playerStats == null)
        {
            playerStats = GetComponentInParent<PlayerStats>();
            if (playerStats == null)
            {
                playerStats = FindAnyObjectByType<PlayerStats>();
            }
        }

        if (currencyHandler == null)
        {
            currencyHandler = GetComponentInParent<CurrencyHandler>();
            if (currencyHandler == null)
            {
                currencyHandler = FindAnyObjectByType<CurrencyHandler>();
            }
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Play) return;

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    void OnAttack()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Play) return;

        if (attackCooldown <= 0)
        {
            attackCooldown = 0.2f;
            if (animator != null)
            {
                animator.SetTrigger("attack");
            }
            if (attackEffect != null)
            {
                attackEffect.enabled = true;
            }
        }
    }

    public void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackBox, 0f, enemyLayers);

        if (hitEnemies.Length > 0 && cam != null)
        {
            cam.Shake(0.2f);
        }

        float finalDamage = baseAttackDmg;
        bool isRevenge = GameManager.Instance != null && GameManager.Instance.CurrentPlayerState == GameManager.PlayerState.Revenge;

        if (isRevenge && playerStats != null)
        {
            int blood = currencyHandler != null ? currencyHandler.GetPeakBlood() : 100;
            RevengeStats stats = playerStats.CalculateRevenge(blood);
            finalDamage *= stats.damage;
        }

        int calculatedDmg = Mathf.RoundToInt(finalDamage);

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(calculatedDmg);
            }
        }

        if (attackEffect != null)
        {
            attackEffect.enabled = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackBox);
    }
}