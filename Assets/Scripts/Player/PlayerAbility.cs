using UnityEngine;
using System.Collections.Generic;

public class PlayerAbility : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private string anim;

    [SerializeField] private ShopHandler shopHandler;
    [SerializeField] private PlayerStats playerStats;    
    [SerializeField] private float damageScaling;

    private HashSet<Collider2D> targets = new HashSet<Collider2D>();

    void Awake()
    {
        animator.Play(anim, 0, 0f);
    }

    void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();
        }

        if (shopHandler == null)
        {
            shopHandler = FindAnyObjectByType<ShopHandler>();
        }
    }


    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            targets.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            targets.Remove(other);
        }
    }

    private void AbilityAtk()
    {
        foreach (Collider2D enemy in targets)
        {
            // Debug.Log($"hit {target}");

            if(enemy.CompareTag("Bullet")) { Destroy(enemy.gameObject); }

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {        
                float meleeDamage = playerStats.getDefaultStat(PlayerStats.Stat.Damage) + (10 * shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Damage));
                float abilityDamage = (damageScaling * shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Ability));
                float finalDamage = meleeDamage + abilityDamage;

                enemyComponent.TakeDamage(Mathf.FloorToInt(finalDamage));
            }
        }
        return;
    }
}
