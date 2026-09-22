using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; }
    private DebugInputs debugInputs;

    void Awake()
    {
        // note to self: awake occurs before start
        debugInputs = new DebugInputs();
        if(Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }
    }

    // void Start()
    // {
    // }

    void OnEnable()
    {
        if (debugInputs == null) 
        {
            Debug.Log("debugInputs reference not found");
            return;
        }
        debugInputs.Debug.Enable();
        debugInputs.Debug.GoForward.performed += OnGoForward;
    }

    void OnDisable()
    {
        if (debugInputs == null) 
        {
            Debug.Log("debugInputs reference not found");
            return;
        }
        debugInputs.Debug.Disable();
        debugInputs.Debug.GoForward.performed -= OnGoForward;
    }

    void OnGoForward(InputAction.CallbackContext context){
        transform.position += new Vector3(250f, 10f, 0f);
    }
}
