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

    [field: SerializeField] public CurrencyHandler Currency { get; private set; }
    [field: SerializeField] public MapHandler Map { get; private set; }
    [field: SerializeField] public PlayerSpawner PlayerSpawner { get; private set; }

    public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnStateChanged;

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
    }

    public void Rebirth()
    {
        if(Currency.GetBlood() <= 50) Currency.SetBlood(50);
        Map.ResetMap();
        SetState(GameState.Play);
    }

    public void SetState(GameState newState)
    {

        if (CurrentState == newState) return;

        CurrentState = newState;

        switch(CurrentState)
        {
            case GameState.Play:
                Time.timeScale = 1.0f;
                break;
            case GameState.Dead:
                Time.timeScale = 0;
                break;
            case GameState.Shop:
                break;
            case GameState.Pause:
                Time.timeScale = 0;
                break;
        }

        OnStateChanged?.Invoke(CurrentState);

    }

    void Update()
    {

    }
}
