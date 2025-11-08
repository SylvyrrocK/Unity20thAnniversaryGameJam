using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UpgradeSystem.Interfaces;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int maxHealth = 3;

    [Header("Directional Sprites")]
    [SerializeField] Sprite downSprite;
    [SerializeField] Sprite upSprite;
    [SerializeField] Sprite leftSprite;
    [SerializeField] Sprite rightSprite;

    private SpriteRenderer spriteRenderer;

    [Header("UI Elements")]
    [SerializeField] Image[] hearts;
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;
    //[SerializeField] Sprite halfHeart;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerRb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        playerRb.linearVelocity = new Vector2(_horizontal * moveSpeed, _vertical * moveSpeed);

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < Mathf.FloorToInt(_currentHealth))
            {
                hearts[i].sprite = fullHeart;
            }
            //else if (i < _currentHealth)
            //{
            //    hearts[i].sprite = halfHeart;
            //}
            else
            {
                hearts[i].sprite = emptyHeart;
            }
            hearts[i].enabled = i < Mathf.CeilToInt(maxHealth);

            if (i < Mathf.FloorToInt(maxHealth))
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }

        UpdateSpriteDirection();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
        _vertical = context.ReadValue<Vector2>().y;
    }

    void UpdateSpriteDirection()
    {
        if (playerRb.linearVelocityY > 0) spriteRenderer.sprite = upSprite;
        else if (playerRb.linearVelocityY < 0) spriteRenderer.sprite = downSprite;
        else if (playerRb.linearVelocityX < 0) spriteRenderer.sprite = leftSprite;
        else if (playerRb.linearVelocityX > 0) spriteRenderer.sprite = rightSprite;
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
