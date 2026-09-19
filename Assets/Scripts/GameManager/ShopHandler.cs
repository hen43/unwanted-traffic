using System.Collections.Generic;
using UnityEngine;

public class ShopHandler : MonoBehaviour
{
    [SerializeField] private CurrencyHandler currencyHandler;
    [SerializeField] private Canvas upgrades;
    [SerializeField] private GameObject rebirth;

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

    private void OnEnable()
    {
        GameManager.OnStateChanged += shopOpenCheck;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= shopOpenCheck;
    }

    void Start()
    {
        if(currencyHandler == null)
        {
            currencyHandler = GameManager.Instance.Currency;
        }

        LoadMultipliers();
        upgrades.enabled = false;
        rebirth.SetActive(true);
    }

    void Update()
    {
        
    }

    private void shopOpenCheck(GameManager.GameState state)
    {
        if(state == GameManager.GameState.Shop)
        {
            upgrades.enabled = true;
            rebirth.SetActive(false);
        }
        else
        {
            upgrades.enabled = false;
            if(state == GameManager.GameState.Play)
            {
                rebirth.SetActive(true);
            } 
            else
            {
                rebirth.SetActive(false);
            }
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

    public void UpgradeDamage()
    {
        UpgradeItem(0);
    }

    public void UpgradeSpeed()
    {
        UpgradeItem(1);
    }

    public void UpgradeAbility()
    {
        UpgradeItem(2);
    }

    private void UpgradeItem(int upgradeIndex)
    {
        Upgrade upgrade = (Upgrade)upgradeIndex;

        switch (upgrade)
        {
            case Upgrade.Damage:
                currentUpgrade[Upgrade.Damage]++;
                break;
            case Upgrade.Speed:
                currentUpgrade[Upgrade.Speed]++;
                break;
            case Upgrade.Ability:
                currentUpgrade[Upgrade.Ability]++;
                break;
        }
        SaveMultipliers();
    }

    public int GetMultiplier(Upgrade multiplier)
    {
        return currentUpgrade.ContainsKey(multiplier) ? currentUpgrade[multiplier] : defaultUpgrade[multiplier];
    }

    public void ResetMultipliers()
    {
        foreach (Upgrade upgrade in System.Enum.GetValues(typeof(Upgrade)))
        {
            string key = $"Mult_{upgrade}";
            PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
        LoadMultipliers();
    }
}