using UnityEngine;

public class AbilityHelper : MonoBehaviour
{

    private ShopHandler shopHandler;

    [SerializeField] private Canvas canvas;
    private float timer;

    [SerializeField] private Animator animator;

    public void Hide() => canvas.enabled = false;
    public void Show(){
        canvas.enabled = true;
        timer = 5f;
    }

    void Awake()
    {
        if (shopHandler == null)
        {
            shopHandler = FindAnyObjectByType<ShopHandler>();
        }
    }

    void OnEnable()
    {
        ShopHandler.OnUpgradePurchased += Upgraded;
    }

    void OnDisable()
    {
        ShopHandler.OnUpgradePurchased -= Upgraded;
    }

    void Start()
    {
        Hide();
    }

    private void Upgraded(ShopHandler.Upgrade upg)
    {
        if(upg == ShopHandler.Upgrade.Ability)
        { 
            // Debug.Log("ABILITY UPGRADED");
            Show();
            PlayAnimation();
        } else {
            // Debug.Log("Huhhh...");
        }
    }

    private void PlayAnimation()
    {
        if (animator != null && shopHandler != null)
        {
            animator.SetInteger("IconType", shopHandler.GetUpgradeCount(ShopHandler.Upgrade.Ability));
        }
    }

    void Update()
    {
        if( timer >= 0f ){
            timer -= Time.unscaledDeltaTime;
        } else {
            Hide();
        }
    }
}