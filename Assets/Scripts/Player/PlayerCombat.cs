using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Animator animator;
    private PlayerMovement playerMovement;
    private CameraMovement cam;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private ShopHandler shopHandler;
    [SerializeField] private CurrencyHandler currencyHandler;

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

        float finalDamage = playerStats.getDefaultStat(PlayerStats.Stat.Damage) + (10 * shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Damage));
        bool isRevenge = GameManager.Instance != null && GameManager.Instance.CurrentPlayerState == GameManager.PlayerState.Revenge;

        if (isRevenge && playerStats != null)
        {
            int blood = currencyHandler != null ? currencyHandler.StartingRevengeBlood : 100;
            RevengeStats stats = playerStats.CalculateRevenge(blood);
            finalDamage *= stats.damage;
        }

        int calculatedDmg = Mathf.RoundToInt(finalDamage);

        foreach (Collider2D enemy in hitEnemies)
        {

            if(enemy.CompareTag("Bullet"))
            {
                // call this a parry later and give it effects
                Debug.Log("broke bullet i think");
                Destroy(enemy.gameObject);
            }

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(calculatedDmg);
                Debug.Log($"DAMAGE: {calculatedDmg} with {shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Damage)} upgrades");
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