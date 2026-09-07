using UnityEngine;

public class UIShake : MonoBehaviour
{
    public static UIShake instance;

    [SerializeField] private RectTransform uiElement;

    private Vector2 defaultAnchor;
    private float shakeXRange = 0.0f;
    private float shakeYRange = 0.0f;

    void Awake(){
        instance = this;
    }

    void Start()
    {
        if (uiElement == null) uiElement = GetComponent<RectTransform>();
        if (uiElement != null) defaultAnchor = uiElement.anchoredPosition;
    }

    public void Shake(float x, float y){
        shakeXRange = x;
        shakeYRange = y;
    }

    void LateUpdate()
    {   
        if (uiElement == null) return;

        if (shakeXRange < 0.01f && shakeYRange < 0.01f)
        {
            shakeXRange = 0f;
            shakeYRange = 0f;
            uiElement.anchoredPosition = defaultAnchor;
            return;
        }

        shakeXRange *= 0.96f;
        shakeYRange *= 0.96f;
        
        float shakeX = Random.Range(-shakeXRange, shakeXRange);
        float shakeY = Random.Range(-shakeYRange, shakeYRange);

        uiElement.anchoredPosition = defaultAnchor + new Vector2(shakeX, shakeY);
    }
}