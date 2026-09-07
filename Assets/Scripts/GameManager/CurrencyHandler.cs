using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CurrencyHandler : MonoBehaviour
{
    public int CurrentBlood { get; private set; }
    public int PeakBlood { get; private set; }

    public event Action<int, int> OnBloodChanged;

    private const string BloodSaveKey = "PlayerBlood";
    private const string PeakBloodSaveKey = "PlayerPeakBlood";

    private DebugInputs debugInputs;

    private void Awake()
    {
        LoadBlood();
        debugInputs = new DebugInputs();
    }

    private void OnEnable()
    {
        if (debugInputs == null) return;

        debugInputs.Debug.Enable();

        debugInputs.Debug.GainBlood.performed += OnGainBlood;
        debugInputs.Debug.LoseBlood.performed += OnLoseBlood;
        debugInputs.Debug.SpendBlood.performed += OnSpendBlood;
    }

    private void OnDisable()
    {
        if (debugInputs == null) return;

        debugInputs.Debug.GainBlood.performed -= OnGainBlood;
        debugInputs.Debug.LoseBlood.performed -= OnLoseBlood;
        debugInputs.Debug.SpendBlood.performed -= OnSpendBlood;

        debugInputs.Debug.Disable();
    }

    public void ChangeBlood(int amt)
    {
        CurrentBlood += amt;

        if (CurrentBlood < 0) 
        {
            CurrentBlood = 0;
            GameManager.Instance.SetState(GameManager.GameState.Dead);
        }
        else if (CurrentBlood > PeakBlood)
        {
            PeakBlood = CurrentBlood;
        }
        
        SaveBlood();
        OnBloodChanged?.Invoke(CurrentBlood, PeakBlood);
    }   

    public bool SpendBlood(int amt)
    {
        if (CurrentBlood >= amt)
        {
            CurrentBlood -= amt;
            PeakBlood -= amt;
            
            SaveBlood();
            OnBloodChanged?.Invoke(CurrentBlood, PeakBlood);
            return true;
        }
        return false;
    }

    public int GetBlood() => CurrentBlood;
    public int GetPeakBlood() => PeakBlood;

    private void LoadBlood()
    {
        CurrentBlood = PlayerPrefs.GetInt(BloodSaveKey, 0);
        PeakBlood = PlayerPrefs.GetInt(PeakBloodSaveKey, CurrentBlood);
    }

    private void SaveBlood()
    {
        PlayerPrefs.SetInt(BloodSaveKey, CurrentBlood);
        PlayerPrefs.SetInt(PeakBloodSaveKey, PeakBlood);
        PlayerPrefs.Save();
    }

    // DEBUG

    private void OnGainBlood(InputAction.CallbackContext ctx)
    {
        ChangeBlood(50);
    }

    private void OnLoseBlood(InputAction.CallbackContext ctx)
    {
        ChangeBlood(-50);
    }

    private void OnSpendBlood(InputAction.CallbackContext ctx)
    {
        SpendBlood(50);
    }
}