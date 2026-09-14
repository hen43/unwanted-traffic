using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    public enum RMult
    {
        DamageReduction,
        Damage,
        Speed,
        Jump
    }

    private Dictionary<RMult, float> defaultMult = new Dictionary<RMult, float>
    {
        { RMult.DamageReduction, 1.0f },
        { RMult.Damage,          1.0f },
        { RMult.Speed,           1.0f },
        { RMult.Jump,            1.0f }
    };

    private Dictionary<RMult, float> revengeMult = new Dictionary<RMult, float>
    {
        // The revenge multipliers should be upgradable at some point.
        // However, the main scaling will be blood at revenge.
        { RMult.DamageReduction, 2.0f },
        { RMult.Damage,          2.5f },
        { RMult.Speed,           1.75f },
        { RMult.Jump,            1.5f }
    };

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
