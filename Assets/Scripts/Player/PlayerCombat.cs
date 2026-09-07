using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Animator animator;
    private PlayerMovement playerMovement;
    private CameraMovement cam;

    private int attackDmg = 35;
    public Vector2 attackBox;
    private float attackCooldown = 0f;
    public SpriteRenderer attackEffect;

    void Start(){
        playerMovement = GetComponentInParent<PlayerMovement>();
        cam = CameraMovement.instance;
        attackEffect.enabled = false;
    }

    void Update()
    {
        if(attackCooldown > 0){
            attackCooldown -= Time.deltaTime;
        }
    }

    void OnAttack(){
        if(attackCooldown <= 0){
            attackCooldown = 0.2f;
            animator.SetTrigger("attack");
            attackEffect.enabled = true;
        }
    }

    public void Attack(){
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackBox, 0f, enemyLayers);

        if(hitEnemies.Length > 0){
            cam.Shake(0.2f);
        }

        foreach(Collider2D enemy in hitEnemies){
            // Debug.Log("hit" + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(attackDmg);
        }

        attackEffect.enabled = false;
    }

    void OnDrawGizmosSelected(){
        if(attackPoint == null){
            return;
        }
        Gizmos.DrawWireCube(attackPoint.position, attackBox);
    }
}