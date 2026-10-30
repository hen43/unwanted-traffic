using UnityEngine;
using UnityEngine.EventSystems;

public class UISizeHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool mouseHovering = false;
    private float initScaleX;
    private float initScaleY;
    private RectTransform rectTransform;
    [SerializeField] private float scaleSpeed = 6f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initScaleX = rectTransform.localScale.x;
        initScaleY = rectTransform.localScale.y;
    }
    
    public void OnPointerEnter(PointerEventData data)
    {
        mouseHovering = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        mouseHovering = false;
    }

    void Update()
    {
        Vector3 scale = rectTransform.localScale;

        if (mouseHovering)
        {
            scale.x += ((initScaleX * 1.1f) - scale.x) / scaleSpeed;
            scale.y += ((initScaleY * 1.1f) - scale.y) / scaleSpeed;
        }
        else
        {
            scale.x += ((initScaleX * 1.0f) - scale.x) / scaleSpeed;
            scale.y += ((initScaleY * 1.0f) - scale.y) / scaleSpeed;
        }

        rectTransform.localScale = scale;
    }
}
