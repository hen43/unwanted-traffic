using UnityEngine;

public class Conehead3D : MonoBehaviour
{

    public float sineMag = 10f; 
    public float sineSpeed = 10f;

    public float hzSpeed =200f;
    private Vector3 startRotation = new Vector3(0f, 0f, 0f);
    private float elapsedTime;

    private void Start(){
        transform.rotation = Quaternion.Euler(startRotation);
    }

    void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;
        Vector3 rotationSpeed = new Vector3(Mathf.Sin(elapsedTime * sineSpeed) * sineMag, hzSpeed, 0f);
        transform.Rotate(rotationSpeed * Time.unscaledDeltaTime);
    }
}
