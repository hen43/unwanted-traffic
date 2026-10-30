using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxFactor;
    [SerializeField] private float initY; //yay magic numbers
    private SpriteRenderer rend;
    private Camera cam;
    [SerializeField] private CameraMovement camScript;

    void Start()
    {
        cam = GetComponentInParent<Camera>();        
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, initY + cam.transform.position.y * parallaxFactor, transform.position.z);
        rend.color = Color.Lerp(rend.color, camScript.targetColor, Time.deltaTime);
    }
}
