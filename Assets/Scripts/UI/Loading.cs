using UnityEngine;

public class Loading : MonoBehaviour
{

    [SerializeField] private Canvas canvas;
    private float loadingTime = 3f;

    void Start()
    {
        canvas.enabled = true;
    }

    void Update()
    {
        if(loadingTime >= 0f)
        {
            loadingTime -= Time.unscaledDeltaTime;
        } else {
            canvas.enabled = false;
        }
    }
}
