using UnityEngine;
using System.Collections.Generic;

public struct RevengeStats
{
    public float damageReduction;
    public float damage;
    public float speed;
    public float jump;
    public float decayRate;
}

public class PlayerStats : MonoBehaviour, ITooltipDataProvider
{

    [SerializeField] private ShopHandler shopHandler;

    public enum RMult
    {
        DamageReduction,
        Damage,
        Speed,
        Jump
    }

    public enum Stat
    {
        Damage,
        Speed,
    }
    

    public void Start()
    {
        if(shopHandler == null)
        {
            shopHandler = FindAnyObjectByType<ShopHandler>();
        }
    }

    private Dictionary<Stat, float> defaultStats = new Dictionary<Stat, float>
    {
        { Stat.Damage, 25f },
        { Stat.Speed, 15f },
    };

    private Dictionary<RMult, float> defaultMult = new Dictionary<RMult, float>
    {
        { RMult.DamageReduction, 1.0f },
        { RMult.Damage,          1.0f },
        { RMult.Speed,           1.0f },
        { RMult.Jump,            1.0f }
    };

    private Dictionary<RMult, float> revengeMult = new Dictionary<RMult, float>
    {
        { RMult.DamageReduction, 1.0f },
        { RMult.Damage,          1.5f },
        { RMult.Speed,           1.0f },
        { RMult.Jump,            1.0f }
    };

    [Header("Dependencies")]
    [SerializeField] private CurrencyHandler currencyHandler;

    public float getDefaultStat(Stat stat)
    {
        return defaultStats[stat];
    }

    private void Awake()
    {
        if (currencyHandler == null)
        {
            currencyHandler = GetComponent<CurrencyHandler>();
            if (currencyHandler == null)
            {
                currencyHandler = FindAnyObjectByType<CurrencyHandler>();
            }
        }
    }

    public RevengeStats CalculateRevenge(int blood, float decayRate = 0f)
    {
        float dmgRd = blood / 3000f;
        float dmg = blood / 1000f;
        float speed = blood / 2500f;
        float jump = blood / 4000f;
        float reductionFactor = Mathf.Max(0.2f, 1.0f - dmgRd);

        return new RevengeStats
        {
            damageReduction = reductionFactor,
            damage = dmg + revengeMult[RMult.Damage],
            speed = speed + revengeMult[RMult.Speed],
            jump = jump + revengeMult[RMult.Jump],
            decayRate = decayRate
        };
    }

    public string GetHeader(string id)
    {
        if (id == "Revenge")
        {
            return "REVENGE";
        }
        else
        {
            return "Unknown Tooltip ID";
        }
    }

    public string GetContent(string id)
    {
        if (id == "Revenge")
        {
            return GetRevengeTooltip();
        }
        else
        {
            return "Unknown Tooltip ID";
        }
    }

    private string GetRevengeTooltip()
    {
        if (currencyHandler == null)
        {
            currencyHandler = FindAnyObjectByType<CurrencyHandler>();
        }

        int activeBlood = currencyHandler != null ? currencyHandler.GetPeakBlood() : 0;
        float decayRate = currencyHandler != null ? currencyHandler.GetRevengeDecayRate() : 0f;

        RevengeStats stats = CalculateRevenge(activeBlood, decayRate);
        
        string dynamicStatsText;

        if(activeBlood <= 100){
            dynamicStatsText = 
                "<b><color=red>REVENGE requires PEAK BLOOD to be GREATER THAN 100</color></b>\n\n";
        } else {
            dynamicStatsText = 
                $"<b>With {activeBlood} Peak Blood:</b>\n" +
                $"Damage Reduction: <color=red>{stats.damageReduction:F2}x</color>\n" +
                $"Damage: <color=green>{stats.damage:F2}x</color>\n" +
                $"Speed: <color=green>{stats.speed:F2}x</color>\n" +
                $"Jump: <color=green>{stats.jump:F2}x</color>\n" +
                $"Drain Rate: <color=red>-{stats.decayRate:F1} Blood/s</color>\n\n";
        }


        string baseDescription = 
            "Continue the run by setting your BLOOD to PEAK BLOOD. " +
            "BLOOD and PEAK BLOOD will drain to 100 in 30 seconds, but the duration is " +
            "extended or lowered from BLOOD gain and loss. In exchange, the player has " +
            "enhanced stats for the duration of REVENGE. All upgrades are kept.";

        return dynamicStatsText + baseDescription;
    }
}