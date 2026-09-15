using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public RectTransform rectTransform;
    public int characterWrapLimit = 80;

    [Header("Edge Detection & Positioning")]
    public float edgeThreshold = 50f;
    public Vector2 tooltipOffset = new Vector2(10f, -10f);

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        GameManager.HideTooltip += HideTooltip;
    }

    private void OnDisable()
    {
        GameManager.HideTooltip -= HideTooltip;
    }

    private void HideTooltip()
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

        // Dynamic wrap checking
        int headerLength = string.IsNullOrEmpty(header) ? 0 : header.Length;
        int contentLength = string.IsNullOrEmpty(content) ? 0 : content.Length;

        if (layoutElement != null)
        {
            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit);
        }
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