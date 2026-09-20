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
            currentUpgrade[upgrade]++;
        }
        else
        {
            currentUpgrade[upgrade] = defaultUpgrade[upgrade] + 1;
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

    public string DamageContent()
    {
        if (Stats == null)
        {
            return "Upgrades your damage by 10 for every upgrade.\n\nCurrent: Loading...";
        }

        float baseDamage = 0f;
        try
        {
            baseDamage = Stats.getDefaultStat(PlayerStats.Stat.Damage);
        }
        catch
        {
            return "Upgrades your damage by 10 for every upgrade.\n\nCurrent: Loading...";
        }

        float dmg = baseDamage + (10 * GetUpgradeCount(Upgrade.Damage));
        return $"Upgrades your damage by 10 for every upgrade.\n\nCurrent: {dmg}";
    }

    public string SpeedContent()
    {
        if (Stats == null)
        {
            return "Upgrades movement speed.\n\nCurrent: Loading...";
        }

        return "speed default text";
    }

    public string AbilityContent()
    {
        if (Stats == null)
        {
            return "Upgrades ability power.\n\nCurrent: Loading...";
        }

        return "ability default text";
    }
}