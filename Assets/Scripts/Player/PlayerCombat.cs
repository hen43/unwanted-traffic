using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public Transform attackPoint_up;
    public Transform attackPoint_down;
    public LayerMask enemyLayers;
    public Animator animator;
    private PlayerMovement playerMovement;
    private CameraMovement cam;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private ShopHandler shopHandler;
    [SerializeField] private CurrencyHandler currencyHandler;    

    public Vector2 attackBox;
    public Vector2 attackBox_alt;
    public Vector3 vectorShift;
    private float attackCooldown = 0f;
    private float moveX;
    private float moveY;

    [SerializeField] private GameObject[] abilityList;

    public SpriteRenderer attackEffect;
    public SpriteRenderer attackEffect_up;
    public SpriteRenderer attackEffect_down;

    float finalDamage;

    private SpriteRenderer activeEffect;

    public enum AbilityType
    {
        Forward,
        Down,
        Back
    }

    [SerializeField] private float forwardShift;
    [SerializeField] private float downShift;
    [SerializeField] private float backShift;

    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        cam = CameraMovement.instance;

        DisableAllEffects();

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

        moveX = playerMovement.moveX;
        moveY = playerMovement.moveY;
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

            DisableAllEffects();

            activeEffect = GetEffectForDirection();
            if (activeEffect != null)
            {
                activeEffect.enabled = true;
            }
        }
    }

    private SpriteRenderer GetEffectForDirection()
    {
        if (moveY == 1) return attackEffect_up;
        if (moveY == -1) return attackEffect_down;
        return attackEffect;
    }

    private void DisableAllEffects()
    {
        if (attackEffect != null) attackEffect.enabled = false;
        if (attackEffect_up != null) attackEffect_up.enabled = false;
        if (attackEffect_down != null) attackEffect_down.enabled = false;
        activeEffect = null;
    }

    public void Attack()
    {
        Collider2D[] hitEnemies;

        if(moveY == 1)
        {
            hitEnemies = Physics2D.OverlapBoxAll(attackPoint_up.position, attackBox_alt, 0f, enemyLayers);
        }
        else if (moveY == -1)
        {
            hitEnemies = Physics2D.OverlapBoxAll(attackPoint_down.position, attackBox_alt, 0f, enemyLayers);
        } 
        else 
        {
            hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackBox, 0f, enemyLayers);
        }

        if (hitEnemies.Length > 0 && cam != null)
        {
            cam.Shake(0.2f);
        }

        finalDamage = 1f;
        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            finalDamage = playerStats.getDefaultStat(PlayerStats.Stat.Damage) + (10 * shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Damage));
        }

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
                // Debug.Log("broke bullet i think");
                Destroy(enemy.gameObject);
            }

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(calculatedDmg);
            }
        }

        if (activeEffect != null)
        {
            activeEffect.enabled = false;
            activeEffect = null;
        }
    }

    public void OnCast()
    {
        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
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
    }

    private void Ability(AbilityType type)
    {
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
                    if(blood >= 500) {
                        currencyHandler.ChangeBlood(-500, true);
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

        Vector3 shift = vectorShift;
        Vector3 shift2 = new Vector3(shift.x, shift.y * -1, shift.z);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackBox);
        Gizmos.DrawWireCube(attackPoint.position + shift, attackBox_alt);
        Gizmos.DrawWireCube(attackPoint.position + shift2, attackBox_alt);
    }
}