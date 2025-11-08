using UnityEngine;
using UnityEngine.InputSystem;
using UpgradeSystem.Interfaces;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField] private PlayerInput playerInput;
    
    private Rigidbody2D playerRb;
    private Vector2 inputVector;
    private Vector2 movementVector;
    
    InputAction moveAction;
    
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();    
    }

    public void OnMove(Object test)
    {
        Debug.Log(test);
        //inputVector = context.ReadValue<Vector2>();
        //movementVector = inputVector * moveSpeed;
    }
    
    void FixedUpdate()
    {
        playerRb.linearVelocity = movementVector;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Pickup>(out var pickup))
        {
            pickup.Interact(this);
        }
    }
}
