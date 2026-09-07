using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }
    }
}
