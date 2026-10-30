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
    [SerializeField] private Color green;
    [SerializeField] private Color red_orange;
    [SerializeField] private Color purple;
    public Color targetColor { get; private set; }

    [SerializeField] private Distance playerDist;

    [SerializeField] private Camera cam;
    private GameObject gas;
    private SpriteRenderer gasRenderer;
    public float cameraX { get; private set; }

    private float startZ;
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        startZ = transform.position.z;
        gas = cam.transform.GetChild(0).gameObject;
        gasRenderer = gas.GetComponent<SpriteRenderer>();
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    public void Shake(float x)
    {
        shakeXRange = x;
        shakeYRange = x;
    }

    public void Shake(float x, float y)
    {
        shakeXRange = x;
        shakeYRange = y;
    }

    void Update(){
        cameraX = cam.transform.position.y;
    }

    void LateUpdate()
    {   
        if (player == null) return;

        shakeXRange *= 0.93f;
        shakeYRange *= 0.93f;
        
        shakeX = Random.Range(-shakeXRange, shakeXRange);
        shakeY = Random.Range(-shakeYRange, shakeYRange);

        targetPos = new Vector3(player.position.x + targetOffset.x + shakeX, player.position.y + targetOffset.y + shakeY, startZ);
        transform.position = targetPos;

        if(playerDist.dist < 500){
            targetColor = green;
            // cam.backgroundColor = Color.Lerp(cam.backgroundColor, green, Time.deltaTime);
        } else if (playerDist.dist < 1000){
            targetColor = red_orange;
            // cam.backgroundColor = Color.Lerp(cam.backgroundColor, red_orange, Time.deltaTime);
        } else {
            targetColor = purple;
            // cam.backgroundColor = Color.Lerp(cam.backgroundColor, purple, Time.deltaTime);
        }

        cam.backgroundColor = Color.Lerp(cam.backgroundColor, targetColor, Time.deltaTime);
        float colorFluc = 0.35f + Mathf.Sin(Time.time * 3f) * 0.2f;
        Color newColor = new Color(targetColor.r, targetColor.g, targetColor.b, colorFluc);
        gasRenderer.color = Color.Lerp(gasRenderer.color, newColor, Time.deltaTime);
    }
}