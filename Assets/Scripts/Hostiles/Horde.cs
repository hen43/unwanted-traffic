using UnityEngine;
using System.Collections;

public class Horde : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float stayMult = 1;
    private float stayTimer = 0;
    private CurrencyHandler currencyHandler; 
    private CameraMovement cam;
    
    void Start()
    {
        cam = CameraMovement.instance;
        if(currencyHandler == null)
        {
            currencyHandler = FindFirstObjectByType<CurrencyHandler>();
        }
    }

    void Update()
    {
        gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            stayTimer += Mathf.Pow(stayMult, Time.deltaTime);
            currencyHandler?.ChangeBlood(-1 * (1 + Mathf.FloorToInt(stayTimer)));
            cam.Shake(Mathf.Clamp((stayTimer * 0.1f), 0f, 1f));
        }

        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>()?.EnvironmentDeath();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            stayTimer = 0;
        }
    }
}
