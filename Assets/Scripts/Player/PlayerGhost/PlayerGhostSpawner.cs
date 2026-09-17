using UnityEngine;

public class PlayerGhostSpawner : MonoBehaviour
{
    [Header("Trail Settings")]
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float trailInterval = 0.025f; 
    [SerializeField] private float ghostDuration = 0.5f;   
    [SerializeField] private Color revengeTrailColor = new Color(1f, 0f, 0f, 0.5f); 

    private SpriteRenderer playerSpriteRenderer;
    private float trailTimer;

    private void Awake()
    {
        Transform playerTransform = transform.Find("PlayerSprite");
        playerSpriteRenderer = playerTransform.GetComponent<SpriteRenderer>();
        
        if (playerSpriteRenderer == null)
        {
            Debug.LogError("PlayerGhostSpawner couldn't find a SpriteRenderer on the player!");
        }
    }

    private void Update()
    {
        bool isRevenge = GameManager.Instance != null && 
                         GameManager.Instance.CurrentPlayerState == GameManager.PlayerState.Revenge;

        if (isRevenge)
        {
            trailTimer -= Time.deltaTime;
            if (trailTimer <= 0f)
            {
                SpawnGhost();
                trailTimer = trailInterval;
            }
        }
    }

    private void SpawnGhost()
    {
        if (ghostPrefab == null || playerSpriteRenderer == null) return;

        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
        PlayerGhost ghostComponent = ghost.GetComponent<PlayerGhost>();
        
        if (ghostComponent != null)
        {
            ghostComponent.Initialize(
                playerSpriteRenderer, 
                revengeTrailColor, 
                ghostDuration
            );
        }
    }
}