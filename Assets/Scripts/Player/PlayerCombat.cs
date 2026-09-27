using UnityEngine;
using UnityEngine.InputSystem;

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
    private float moveX;
    private float moveY;

    [SerializeField] private GameObject[] abilityList;

    public SpriteRenderer attackEffect;

    public enum AbilityType
    {
        Forward,
        Down,
        Back
    }

    // for ability transposition
    [SerializeField] private float forwardShift;
    [SerializeField] private float downShift;
    [SerializeField] private float backShift;

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
                // Debug.Log($"DAMAGE: {calculatedDmg} with {shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Damage)} upgrades");
            }
        }

        if (attackEffect != null)
        {
            attackEffect.enabled = false;
        }
    }

    public void OnCast()
    {
        moveX = playerMovement.moveX;
        moveY = playerMovement.moveY;
        // Debug.Log($"CASTED with x {moveX} and y {moveY}");

        if(moveY == -1)
        {
            Ability(AbilityType.Down);
            return;
        }
        if(moveX == -1)
        {
            Ability(AbilityType.Back);
            return;
        }
        Ability(AbilityType.Forward);
        return;
    }

    private void Ability(AbilityType type)
    {
        // Debug.Log($"CASTED as {type}");

        int blood = currencyHandler.GetBlood(); 

        switch(type)
        {
            case AbilityType.Forward:
                if(shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Ability) >= 1){
                    if(blood >= 200) {
                        currencyHandler.ChangeBlood(-200, true);
                        AbilityForward();
                    }
                }
                break;
            case AbilityType.Down:
                if(shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Ability) >= 2){
                    if(blood >= 250) {
                        currencyHandler.ChangeBlood(-250, true);
                        AbilityDown();
                    }
                }
                break;
            case AbilityType.Back:
                if(shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Ability) >= 3){
                    if(blood >= 300) {
                        currencyHandler.ChangeBlood(-300, true);
                        AbilityBack();
                    }
                }
                break;
        }
    }

    private void AbilityForward()
    {
        Vector3 shift = new Vector3(playerMovement.dir * forwardShift, 0, 0); 
        Instantiate(abilityList[0], transform.position + shift, Quaternion.identity);
        return;
    }

    private void AbilityDown()
    {
        Vector3 shift = new Vector3(0, downShift, 0); 
        playerMovement.SetYVel(20f);
        Instantiate(abilityList[1], transform.position + shift, Quaternion.identity);
        return;
    }

    private void AbilityBack()
    {
        Vector3 shift = new Vector3(backShift, 0, 0);
        playerMovement.Launch(100f);
        Instantiate(abilityList[2], transform.position + shift, abilityList[2].transform.rotation);
        return;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackBox);
    }
}