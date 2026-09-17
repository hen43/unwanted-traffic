using UnityEngine;
using UnityEngine.UI;

public class RedFlash : MonoBehaviour
{
    [SerializeField] private CurrencyHandler currencyHandler;

    [SerializeField] private float flashDecaySpeed = 1.0f;
    [SerializeField] private float intensityMultiplier = 4.0f;

    [Header("Revenge Mode Settings")]
    [SerializeField] private float revengeBaseAlpha = 0.4f;

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
            bool isRevenge = GameManager.Instance != null && 
                             GameManager.Instance.CurrentPlayerState == GameManager.PlayerState.Revenge;

            if (isRevenge)
            {
                image.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
            }
            else
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
    }

    private void Update()
    {
        bool isRevenge = GameManager.Instance != null && 
                         GameManager.Instance.CurrentPlayerState == GameManager.PlayerState.Revenge;

        float targetAlpha = isRevenge ? revengeBaseAlpha : 0.0f;

        if (!Mathf.Approximately(image.color.a, targetAlpha))
        {
            float newAlpha = Mathf.MoveTowards(image.color.a, targetAlpha, flashDecaySpeed * Time.deltaTime);
            image.color = new Color(1.0f, 0.0f, 0.0f, newAlpha);
        }
    }
}