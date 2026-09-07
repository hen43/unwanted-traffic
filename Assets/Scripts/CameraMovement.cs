using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement instance;

    public Transform player;
    private Vector3 targetPos;
    public Vector2 targetOffset = new Vector2(0f, 2f);

    private float shakeX = 0.0f;
    private float shakeY = 0.0f;
    private float shakeXRange = 0.0f;
    private float shakeYRange = 0.0f;

    private float startZ;
    
    void Awake(){
        instance = this;
    }

    void Start()
    {
        startZ = transform.position.z;
    
        // PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
    }

    public void Shake(float x){
        shakeXRange = x;
        shakeYRange = x;
    }

    public void Shake(float x, float y){
        shakeXRange = x;
        shakeYRange = y;
    }

    // Update is called once per frame
    void LateUpdate()
    {   
        shakeXRange *= 0.93f;
        shakeYRange *= 0.93f;
        
        shakeX = Random.Range(-shakeXRange, shakeXRange);
        shakeY = Random.Range(-shakeYRange, shakeYRange);

        targetPos = new Vector3(player.position.x + targetOffset.x + shakeX, player.position.y + targetOffset.y + shakeY, startZ);
        transform.position = targetPos;
    }
}
