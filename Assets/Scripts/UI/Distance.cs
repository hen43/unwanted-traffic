using UnityEngine;
using TMPro;

public class Distance : MonoBehaviour
{
    [SerializeField] private CurrencyHandler currencyHandler;

    public TMP_Text distanceText;

    private Player player;
    private float startingX;
    private bool initializedBaseline = false;

    public float dist { get; private set; }

    void Start()
    {
        TryFindPlayer();
    }

    void OnEnable()
    {
        GameManager.OnStateChanged += hide;
    }

    void OnDisable()
    {
        GameManager.OnStateChanged -= hide;
    }

    void hide(GameManager.GameState gameState)
    {
        if(gameState == GameManager.GameState.Shop)
        {
            distanceText.enabled = false;
        } else {
            distanceText.enabled = true;
        }
    }

    void Update()
    {
        if (player == null)
        {
            TryFindPlayer();
            return;
        }

        if (distanceText != null)
        {
            dist = Mathf.FloorToInt(player.transform.position.x - startingX);
            distanceText.text = $"Distance: {dist}m";
        }
    }

    private void TryFindPlayer()
    {
        player = FindAnyObjectByType<Player>();
        if (player != null && !initializedBaseline)
        {
            startingX = player.transform.position.x;
            initializedBaseline = true;
        }
    }
}