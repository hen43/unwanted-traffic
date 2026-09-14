using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public RectTransform rectTransform;
    public int characterWrapLimit;
    
    [Header("Edge Detection")]
    public float edgeThreshold = 50f;
    public Vector2 tooltipOffset = new Vector2(10f, -10f);

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnEnable()
    {
        GameManager.HideTooltip += hideTooltip;
    }

    public void OnDisable()
    {
        GameManager.HideTooltip -= hideTooltip;
    }

    private void hideTooltip()
    {
        gameObject.SetActive(false);
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;
    }

    private void Update()
    {
        if (Mouse.current != null)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 pivot = CalculatePivotWithEdgeDetection(mousePosition);
            rectTransform.pivot = pivot;
            
            Vector2 tooltipPosition = mousePosition + tooltipOffset;
            transform.position = tooltipPosition;
        }

        if (Application.isEditor)
        {
            int headerLength = headerField.text != null ? headerField.text.Length : 0; 
            int contentLength = contentField.text != null ? contentField.text.Length : 0;
            if (layoutElement != null)
            {
                layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit);
            }
        }
    }

    private Vector2 CalculatePivotWithEdgeDetection(Vector2 mousePosition)
    {
        Vector2 pivot = new Vector2(0f, 1f);
        
        float distanceFromRight = Screen.width - mousePosition.x;
        float distanceFromBottom = mousePosition.y;
        
        if (distanceFromRight < edgeThreshold)
        {
            pivot.x = 1f;
        }
        
        if (distanceFromBottom < edgeThreshold)
        {
            pivot.y = 0f;
        }
        
        return pivot;
    }
}