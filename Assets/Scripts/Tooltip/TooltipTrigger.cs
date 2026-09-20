using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Static Content (Default)")]
    public string header;
    [TextArea(3, 10)]
    public string content;

    [Header("Dynamic Content (Optional)")]
    [Tooltip("Drag a script that implements ITooltipDataProvider here for live data updates.")]
    [SerializeField] private MonoBehaviour dynamicProvider;

    private ITooltipDataProvider provider;
    public string tooltipId;

    private bool isHovered = false;

    private void Awake()
    {
        if (dynamicProvider != null)
        {
            provider = dynamicProvider as ITooltipDataProvider;
            if (provider == null)
            {
                Debug.LogWarning($"[TooltipTrigger] Attached script on {gameObject.name} does not implement ITooltipDataProvider.", this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (provider != null)
        {
            TooltipSystem.Show(provider.GetContent(tooltipId), provider.GetHeader(tooltipId));
        }
        else
        {
            TooltipSystem.Show(content, header);
        }
    } 

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }

    private void ShowTooltip()
    {
        if (provider != null)
        {
            TooltipSystem.Show(provider.GetContent(tooltipId), provider.GetHeader(tooltipId));
        }
        else
        {
            TooltipSystem.Show(content, header);
        }
    }

    private void RefreshIfHovered(string updatedId)
    {
        if (isHovered && updatedId == tooltipId)
        {
            ShowTooltip();
        }
    }

    public void HideTooltip()
    {
        TooltipSystem.Hide();
    }

    public void SetDynamicProvider(ITooltipDataProvider newProvider)
    {
        provider = newProvider;
    }
}