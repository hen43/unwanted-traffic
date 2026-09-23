using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public Collider2D col;
    public Rigidbody2D rb;
    public LayerMask walls;
    private CameraMovement cam;
    private CurrencyHandler currencyHandler; 
    private int damage;

    void Start()
    {
        cam = CameraMovement.instance;
        rb.linearVelocity = transform.right * speed;
        currencyHandler = FindAnyObjectByType<CurrencyHandler>();

        Destroy(gameObject, 4f);
    }

    public void Initialize(int damageAmount)
    {
        damage = damageAmount;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("Bullet"))
        {
            return;
        }
        
        if(other.CompareTag("Player"))
        {
            // Debug.Log("hit the player");

            currencyHandler?.ChangeBlood(damage * -1);
            cam.Shake(0.2f);

            Destroy(gameObject);
            return;
        }

        Debug.Log($"hit {other.name}");
        return;
    }
}
