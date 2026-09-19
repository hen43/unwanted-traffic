using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState {
        Play,
        Dead,
        Shop,
        Pause
    }

    public enum PlayerState {
        Normal,
        Revenge
    }

    [field: SerializeField] public CurrencyHandler Currency { get; private set; }
    [field: SerializeField] public MapHandler Map { get; private set; }

    public GameState CurrentState { get; private set; }
    public PlayerState CurrentPlayerState { get; private set; }

    public static event Action<GameState> OnStateChanged;
    public static event Action<PlayerState> PlayerStateChanged;
    public static event Action HideTooltip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
 
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurrentState = GameState.Play;
        CurrentPlayerState = PlayerState.Normal;
    }

    public void Rebirth()
    {
        // Debug.Log("Rebirth initiated.");
        Map.ResetMap();
        SetGameState(GameState.Shop);
    }

    public void ExitShop()
    {
        SetGameState(GameState.Play);
        HideTooltip?.Invoke();
    }

    public void Revert()
    {
        if (Currency.GetBlood() <= 100) Currency.SetBlood(100);
        PlayerStateChanged?.Invoke(PlayerState.Normal);
        Map.ResetMap();
        SetGameState(GameState.Play);
        HideTooltip?.Invoke();
    }

    public void Revenge()
    {
        if (Currency.GetPeakBlood() <= 100)
        {
            return;
        }
        Currency.SetBlood(Currency.GetPeakBlood());
        SetPlayerState(PlayerState.Revenge);
        SetGameState(GameState.Play);
        HideTooltip?.Invoke();
    }

    public void SetPlayerState(PlayerState newState)
    {
        if (CurrentPlayerState == newState) return;

        CurrentPlayerState = newState;
        PlayerStateChanged?.Invoke(CurrentPlayerState);
    }

    public void SetGameState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.Play:
                Time.timeScale = 1.0f;
                break;
            case GameState.Dead:
                Time.timeScale = 0;
                break;
            case GameState.Shop:
                Time.timeScale = 0;
                break;
            case GameState.Pause:
                Time.timeScale = 0;
                break;
        }

        OnStateChanged?.Invoke(CurrentState);
    }
}