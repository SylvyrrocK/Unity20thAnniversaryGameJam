using UnityEngine;
using UnityEngine.InputSystem;
using UpgradeSystem.Interfaces;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private Rigidbody2D playerRb;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int maxHealth = 3;
    private float _horizontal;
    private float _vertical;

    private int _currentHealth;
    private bool _isInvincible = false;

    public event System.Action<int> OnHealthChanged;
    public event System.Action OnPlayerDeath;
    
    void Start()
    {
        _currentHealth = maxHealth;
    }
    
    void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();

    }

    void FixedUpdate()
    {
        playerRb.linearVelocity = new Vector2(_horizontal * moveSpeed, _vertical * moveSpeed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Move action performed");
        _horizontal = context.ReadValue<Vector2>().x;
        _vertical = context.ReadValue<Vector2>().y;
    }

    public void OnAttack()
    {
        
    }
    
    public void TakeDamage(int damage)
    {
        if (!_isInvincible)
        {
            _currentHealth -= damage;    
            OnHealthChanged?.Invoke(_currentHealth);

            if (_currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Pickup>(out var pickup))
        {
            pickup.OnPickup(this);
        } else if (collision.TryGetComponent<IDamageDealer>(out var damager))
        {
            damager.ApplyDamage(this);
        }
    }
}
