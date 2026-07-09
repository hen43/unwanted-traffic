using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    private Vector3 targetPos;
    public Vector2 targetOffset = new Vector2(0f, 2f);

    private float shakeX = 0.0f;
    private float shakeY = 0.0f;
    private float shakeXRange = 0.0f;
    private float shakeYRange = 0.0f;

    private float startZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startZ = transform.position.z;
    
        // PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
    }
    
    // Update is called once per frame
    void LateUpdate()
    {   
        shakeXRange *= 0.96f;
        shakeYRange *= 0.96f;
        
        shakeX = Random.Range(-shakeXRange, shakeXRange);
        shakeY = Random.Range(-shakeYRange, shakeYRange);

        targetPos = new Vector3(player.position.x + targetOffset.x + shakeX, player.position.y + targetOffset.y + shakeY, startZ);
        transform.position = targetPos;
    }
}
