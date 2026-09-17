using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    private SpriteRenderer ghostSpriteRenderer;
    private float fadeSpeed;
    private Color color;

    public void Initialize(SpriteRenderer targetSpriteRenderer, Color baseColor, float duration)
    {
        ghostSpriteRenderer = GetComponent<SpriteRenderer>();

        ghostSpriteRenderer.sprite = targetSpriteRenderer.sprite;
        ghostSpriteRenderer.flipX = targetSpriteRenderer.flipX;
        ghostSpriteRenderer.flipY = targetSpriteRenderer.flipY;

        ghostSpriteRenderer.sortingLayerID = targetSpriteRenderer.sortingLayerID;
        ghostSpriteRenderer.sortingOrder = targetSpriteRenderer.sortingOrder - 1;

        transform.localScale = targetSpriteRenderer.transform.lossyScale;

        color = baseColor;
        ghostSpriteRenderer.color = color;
        fadeSpeed = 1f / duration;
    }

    private void Update()
    {
        color.a -= fadeSpeed * Time.deltaTime;
        ghostSpriteRenderer.color = color;

        if (color.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}