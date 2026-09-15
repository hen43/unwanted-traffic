using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CurrencyHandler : MonoBehaviour
{
    public int CurrentBlood { get; private set; }
    public int PeakBlood { get; private set; }
    
    private float maxRevengeDuration = 30f;
    private float revengeIncrement;
    private float accumulatedDrain;

    public event Action<int, int, int> OnBloodChanged;

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
        GameManager.PlayerStateChanged += OnPlayerStateChanged;

        if (debugInputs == null) return;
        debugInputs.Debug.Enable();
        debugInputs.Debug.GainBlood.performed += OnGainBlood;
        debugInputs.Debug.LoseBlood.performed += OnLoseBlood;
        debugInputs.Debug.SpendBlood.performed += OnSpendBlood;
    }

    private void OnDisable()
    {
        GameManager.PlayerStateChanged -= OnPlayerStateChanged;

        if (debugInputs == null) return;
        debugInputs.Debug.GainBlood.performed -= OnGainBlood;
        debugInputs.Debug.LoseBlood.performed -= OnLoseBlood;
        debugInputs.Debug.SpendBlood.performed -= OnSpendBlood;
        debugInputs.Debug.Disable();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayerState == GameManager.PlayerState.Revenge)
        {
            accumulatedDrain += revengeIncrement * Time.deltaTime;

            int amountToDrain = Mathf.FloorToInt(accumulatedDrain);
            if (amountToDrain > 0)
            {
                accumulatedDrain -= amountToDrain;
                PeakBlood -= amountToDrain;
                ChangeBlood(-amountToDrain);
            }

            if (CurrentBlood <= 100)
            {
                SetBlood(100);
                GameManager.Instance.SetPlayerState(GameManager.PlayerState.Normal);
            }
        }
    }

    private void OnPlayerStateChanged(GameManager.PlayerState playerState)
    {
        if (playerState == GameManager.PlayerState.Revenge)
        {
            float difference = GetPeakBlood() - 100f;
            revengeIncrement = difference / maxRevengeDuration;
            accumulatedDrain = 0f;
        }
    }

    public void ChangeBlood(int deltaBlood)
    {
        if (deltaBlood < 0 && GameManager.Instance != null && GameManager.Instance.CurrentPlayerState == GameManager.PlayerState.Revenge)
        {
            PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();
            if (playerStats != null)
            {
                RevengeStats stats = playerStats.CalculateRevenge(PeakBlood);
                deltaBlood = Mathf.RoundToInt(deltaBlood / stats.damageReduction);
            }
        }

        CurrentBlood += deltaBlood;

        if (GameManager.Instance != null && GameManager.Instance.CurrentPlayerState == GameManager.PlayerState.Revenge)
        {
            if (deltaBlood < 0)
            {
                PeakBlood += deltaBlood;
            }
        }

        if (CurrentBlood <= 0) 
        {
            CurrentBlood = 0;
            GameManager.Instance.SetGameState(GameManager.GameState.Dead);
        }
        else if (CurrentBlood > PeakBlood)
        {
            PeakBlood = CurrentBlood;
        }

        SaveBlood();
        OnBloodChanged?.Invoke(CurrentBlood, PeakBlood, deltaBlood);
    }

    public bool SpendBlood(int amt)
    {
        if (CurrentBlood >= amt)
        {
            CurrentBlood -= amt;
            PeakBlood -= amt;

            SaveBlood();
            OnBloodChanged?.Invoke(CurrentBlood, PeakBlood, -amt);
            return true;
        }
        return false;
    }

    public void SetBlood(int amt)
    {
        int deltaBlood = amt - CurrentBlood;
        CurrentBlood = amt;

        if (CurrentBlood < 0) 
        {
            CurrentBlood = 0;
            GameManager.Instance.SetGameState(GameManager.GameState.Dead);
        }
        else if (CurrentBlood > PeakBlood)
        {
            PeakBlood = CurrentBlood;
        }

        SaveBlood();
        OnBloodChanged?.Invoke(CurrentBlood, PeakBlood, deltaBlood);
    }

    public int GetBlood() => CurrentBlood;
    public int GetPeakBlood() => PeakBlood;

    public float GetRevengeDecayRate()
    {
        float difference = GetPeakBlood() - 100f;
        if (difference <= 0f) return 0f;
        
        return difference / maxRevengeDuration;
    }

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

    private void OnGainBlood(InputAction.CallbackContext ctx) => ChangeBlood(50);
    private void OnLoseBlood(InputAction.CallbackContext ctx) => ChangeBlood(-50);
    private void OnSpendBlood(InputAction.CallbackContext ctx) => SpendBlood(50);
}