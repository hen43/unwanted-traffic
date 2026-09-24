using System.Collections.Generic;
using UnityEngine;

public class ShopHandler : MonoBehaviour, ITooltipDataProvider
{
    [SerializeField] private CurrencyHandler currencyHandler;
    [SerializeField] private Canvas upgrades;
    [SerializeField] private GameObject rebirth;
    [SerializeField] private PlayerStats playerStats;
    private Player player;

    public static System.Action<string> OnUpgradePurchased;

    public enum Upgrade
    {
        Damage,
        Speed,
        Ability
    }

    private Dictionary<Upgrade, int> defaultUpgrade = new Dictionary<Upgrade, int>
    {
        { Upgrade.Damage, 0 },
        { Upgrade.Speed, 0 },
        { Upgrade.Ability, 0 }
    };

    private Dictionary<Upgrade, int> currentUpgrade = new Dictionary<Upgrade, int>();

    private PlayerStats Stats
    {
        get
        {
            if (playerStats == null)
            {
                player = FindAnyObjectByType<Player>();
                if (player != null)
                {
                    playerStats = player.GetComponent<PlayerStats>();
                }
            }
            return playerStats;
        }
    }

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleGameStateChanged;
        GameManager.PlayerStateChanged += HandlePlayerStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleGameStateChanged;
        GameManager.PlayerStateChanged -= HandlePlayerStateChanged;
    }

    private void Start()
    {
        if (currencyHandler == null && GameManager.Instance != null)
        {
            currencyHandler = GameManager.Instance.Currency;
        }

        LoadMultipliers();
        upgrades.enabled = false;

        if (GameManager.Instance != null)
        {
            UpdateUI(GameManager.Instance.CurrentState, GameManager.Instance.CurrentPlayerState);
        }
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        UpdateUI(state, GameManager.Instance.CurrentPlayerState);
    }

    private void HandlePlayerStateChanged(GameManager.PlayerState playerState)
    {
        UpdateUI(GameManager.Instance.CurrentState, playerState);
    }

    private void UpdateUI(GameManager.GameState gameState, GameManager.PlayerState playerState)
    {
        if (gameState == GameManager.GameState.Shop)
        {
            upgrades.enabled = true;
            rebirth.SetActive(false);
        }
        else
        {
            upgrades.enabled = false;

            bool shouldShowRebirth = (gameState == GameManager.GameState.Play) && (playerState != GameManager.PlayerState.Revenge);
            rebirth.SetActive(shouldShowRebirth);
        }
    }

    private void LoadMultipliers()
    {
        currentUpgrade.Clear();
        foreach (Upgrade upgrade in System.Enum.GetValues(typeof(Upgrade)))
        {
            string key = $"Mult_{upgrade}";
            currentUpgrade[upgrade] = PlayerPrefs.GetInt(key, defaultUpgrade[upgrade]);
        }
    }

    private void SaveMultipliers()
    {
        foreach (var kvp in currentUpgrade)
        {
            string key = $"Mult_{kvp.Key}";
            PlayerPrefs.SetInt(key, kvp.Value);
        }
        PlayerPrefs.Save();
    }

    public void UpgradeDamage() => UpgradeItem(Upgrade.Damage);
    public void UpgradeSpeed() => UpgradeItem(Upgrade.Speed);
    public void UpgradeAbility() => UpgradeItem(Upgrade.Ability);

    private void UpgradeItem(Upgrade upgrade)
    {
        if (currentUpgrade.ContainsKey(upgrade))
        {
            int cost;
            switch(upgrade)
            {
                case(Upgrade.Damage):
                    cost = 300 + (200 * GetUpgradeCount(Upgrade.Damage));
                    if(Spend(cost)){currentUpgrade[upgrade]++;};
                    break;
                case(Upgrade.Speed):
                    cost = 500 + (250 * GetUpgradeCount(Upgrade.Speed));
                    if(Spend(cost)){currentUpgrade[upgrade]++;};
                    break;
                case(Upgrade.Ability):
                    if(GetUpgradeCount(Upgrade.Ability) < 4)
                    {
                        cost = 1000 + (1000 * GetUpgradeCount(Upgrade.Ability));
                        if(Spend(cost)){currentUpgrade[upgrade]++;};
                    }
                    break;  
            }
        } 
        else
        {
            currentUpgrade[upgrade] = defaultUpgrade[upgrade];
        }

        SaveMultipliers();
        
        OnUpgradePurchased?.Invoke(upgrade.ToString());

        TooltipSystem.Show(GetContent(upgrade.ToString()), GetHeader(upgrade.ToString()));
    }

    private void UpgradeItem(int upgradeIndex)
    {
        UpgradeItem((Upgrade)upgradeIndex);
    }

    public int GetUpgradeCount(Upgrade upgrade)
    {
        return currentUpgrade.ContainsKey(upgrade) ? currentUpgrade[upgrade] : defaultUpgrade[upgrade];
    }

    public string GetContent(string id)
    {
        if (id == "Damage") return DamageContent();
        if (id == "Speed") return SpeedContent();
        if (id == "Ability") return AbilityContent();
        return "nah";
    }

    public string GetHeader(string id)
    {
        switch (id)
        {
            case "Damage": return "DAMAGE";
            case "Speed": return "SPEED";
            case "Ability": return "ABILITY";
            default: return "id not found";
        }
    }

    public bool Spend(int amt)
    {
        bool check1 = currencyHandler.GetPeakBlood() >= amt;
        bool check2 = currencyHandler.GetBlood() >= amt; 
        if(check1 && check2)
        {
            currencyHandler.SpendBlood(amt);
            return true;
        } else {
            return false;
        }
    }

    public string DamageContent()
    {
        float dmg = playerStats.getDefaultStat(PlayerStats.Stat.Damage) + (20 * GetUpgradeCount(Upgrade.Damage));
        int cost = 300 + (200 * GetUpgradeCount(Upgrade.Damage));
        return "" + 
        $"Upgrades your damage by 20 for every upgrade.\n\n" + 
        $"Current: {dmg}\n" +
        $"Cost: {FormatCost(cost)}";
    }

    public string SpeedContent()
    {
        float spd = playerStats.getDefaultStat(PlayerStats.Stat.Speed) + (5 * GetUpgradeCount(Upgrade.Speed));
        int cost = 500 + (250 * GetUpgradeCount(Upgrade.Speed));
        return "" + 
        $"Upgrades your speed by 5 for every upgrade.\n\n" + 
        $"Current: {spd}\n" +
        $"Cost: {FormatCost(cost)}";
    }

    public string AbilityContent()
    {
        float abil = GetUpgradeCount(Upgrade.Ability);
        int cost = 1000 + (1000 * (int)abil);
        if(abil >= 3){
            cost = 9999999;
        }
        return "" + 
        $"Unlocks a new ability.\n\n" + 
        $"Current: {abil}/4\n" +
        $"Cost: {FormatCost(cost)}\n\n" + 
        "REVENGE upgrades are unlocked at MAX Ability Upgrades.";
    }

    private string FormatCost(int cost)
    {
        bool canAfford = (currencyHandler.GetBlood() >= cost && currencyHandler.GetPeakBlood() >= cost); 
        
        if (canAfford)
        {
            return cost.ToString();
        }
        else
        {
            return $"<color=red>{cost}</color>";
        }
    }
}