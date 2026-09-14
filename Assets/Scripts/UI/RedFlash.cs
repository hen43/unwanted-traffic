using UnityEngine;
using UnityEngine.UI;

public class RedFlash : MonoBehaviour
{
    [SerializeField] private CurrencyHandler currencyHandler;

    [SerializeField] private float flashDecaySpeed = 1.0f;
    [SerializeField] private float intensityMultiplier = 4.0f;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (currencyHandler != null)
        {
            currencyHandler.OnBloodChanged += RedFlashEffect;
        }
    }

    private void OnDisable()
    {
        if (currencyHandler != null)
        {
            currencyHandler.OnBloodChanged -= RedFlashEffect;
        }
    }

    private void Start()
    {
        image.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
    }

    private void RedFlashEffect(int current, int max, int deltaBlood)
    {
        if (deltaBlood < 0)
        {
            int previousBlood = current - deltaBlood;
            
            if (previousBlood > 0)
            {
                float damagePercent = (float)Mathf.Abs(deltaBlood) / previousBlood;
                float effectStrength = Mathf.Clamp01(damagePercent * intensityMultiplier);
                
                image.color = new Color(1.0f, 0.0f, 0.0f, effectStrength);
            }
        }
    }

    private void Update()
    {
        if (image.color.a > 0.0f)
        {
            float newAlpha = Mathf.MoveTowards(image.color.a, 0f, flashDecaySpeed * Time.deltaTime);
            image.color = new Color(1.0f, 0.0f, 0.0f, newAlpha);
        }
    }
}