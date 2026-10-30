using UnityEngine;
using UnityEngine.UI; 

public class Flicker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float time = 0f;
    private Image img;
    private Color newColor;

    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time > 0.05f){
            time -= 0.05f;
            DoFlicker();
        }    
    }

    void DoFlicker(){
        newColor = new Color(1.0f, 0.5943396f, 0.9251143f, UnityEngine.Random.Range(0.3f, 1.0f));
        img.color = newColor;
    }
}
