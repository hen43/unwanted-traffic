using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private InputAction moveAction;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private float spriteSize = 0.54f;
    [SerializeField] private Transform spriteTf; 

    public float speed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move"); 
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    // Update is called once per frame
    void Update()
    {
        float xVel = moveInput.x * speed;
        float yVel = rb.linearVelocity.y;

        if (xVel != 0){
            spriteTf.localScale = new Vector2(xVel > 0 ? -spriteSize:spriteSize, spriteSize);
        }

        rb.linearVelocity = new Vector2(xVel, yVel);
    }
}

