using UnityEngine;

public class Horde : MonoBehaviour
{
    [SerializeField] private float stayMult = 1;
    public float speed;
    public float speedIncreaseDecay = 10f;

    private float stayTimer = 0f;
    private CurrencyHandler currencyHandler; 
    private CameraMovement cam;

    void Start()
    {
        cam = CameraMovement.instance;
        if (currencyHandler == null)
        {
            currencyHandler = FindAnyObjectByType<CurrencyHandler>();
        }
    }

    private void OnEnable()
    {
        GameManager.PlayerStateChanged += revengeSetback;
    }

    private void OnDisble()
    {
        GameManager.PlayerStateChanged -= revengeSetback;
    }

    void Update()
    {
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);

        if (speed <= 14)
        {
            speed += (Time.deltaTime / speedIncreaseDecay);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            stayTimer += Mathf.Pow(stayMult, Time.deltaTime);
            currencyHandler?.ChangeBlood(-1 * (1 + Mathf.FloorToInt(stayTimer)));
            cam.Shake(Mathf.Clamp((stayTimer * 0.1f), 0f, 1f));
        }

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>()?.EnvironmentDeath();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            stayTimer = 0;
        }
    }

    public void revengeSetback(GameManager.PlayerState playerState)
    {
        transform.position += new Vector3(speed * -5, 0, 0);
    }

    public void setSpeed(float inputSpeed)
    {
        speed = inputSpeed;
    }
}