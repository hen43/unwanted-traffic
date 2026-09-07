using UnityEngine;

public class Conehead3D : MonoBehaviour
{

    public Vector3 rotationSpeed = new Vector3(-100f, 75f, 0f); 
    private Vector3 startRotation = new Vector3(0f, 0f, 0f);

    private void Start(){
        transform.rotation = Quaternion.Euler(startRotation);;
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.unscaledDeltaTime);
    }
}
