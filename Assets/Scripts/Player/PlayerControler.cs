using UnityEngine;
using UnityEngine.InputSystem;
using UpgradeSystem.Interfaces;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;

    [SerializeField] float moveSpeed = 5f;

    private float horizontal;
    private float vertical;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();

    }

    void FixedUpdate()
    {
        playerRb.linearVelocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Move action performed");
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Pickup>(out var pickup))
        {
            pickup.Interact(this);
        }
    }
}
