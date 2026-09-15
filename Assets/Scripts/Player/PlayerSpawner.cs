using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public float height = 5f;
    private GameObject activePlayer;

    [SerializeField] private TooltipTrigger revengeTooltipTrigger;

    private void OnEnable()
    {
        MapHandler.OnFirstTileLoaded += SpawnPlayer;
    }

    private void OnDisable()
    {
        MapHandler.OnFirstTileLoaded -= SpawnPlayer;
    }

    public void SpawnPlayer(Vector3 firstTilePos)
    {
        Vector3 spawnPoint = firstTilePos + new Vector3(0, height, 0);
        
        if (activePlayer == null)
        {
            activePlayer = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);

            CurrencyHandler currency = activePlayer.GetComponentInChildren<CurrencyHandler>();
            if (currency == null)
            {
                currency = FindAnyObjectByType<CurrencyHandler>();
            }

            if (currency != null)
            {
                currency.SetBlood(100);
            }
        }
        else
        {
            activePlayer.transform.position = spawnPoint;

            Rigidbody2D rb = activePlayer.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        if (activePlayer != null && CameraMovement.instance != null)
        {
            CameraMovement.instance.SetTarget(activePlayer.transform);
        }

        BindTooltipTrigger();
    }

    private void BindTooltipTrigger()
    {
        if (activePlayer == null) return;

        if (revengeTooltipTrigger == null)
        {
            revengeTooltipTrigger = FindAnyObjectByType<TooltipTrigger>();
        }

        if (revengeTooltipTrigger != null)
        {
            PlayerStats stats = activePlayer.GetComponentInChildren<PlayerStats>();
            if (stats != null)
            {
                revengeTooltipTrigger.SetDynamicProvider(stats);
            }
        }
    }
}