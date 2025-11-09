using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UpgradeSystem.Interfaces;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int maxHealth = 3;

    [Header("Directional Sprites")] [SerializeField]
    Sprite downSprite;

    [SerializeField] Sprite upSprite;
    [SerializeField] Sprite leftSprite;
    [SerializeField] Sprite rightSprite;

    [Header("Bomb Settings")] [SerializeField]
    private GameObject bombPrefab;
    
    [SerializeField] private LayerMask obstacleLayerMask;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D playerRb;

    [Header("UI Elements")]
    [SerializeField] Image[] hearts;
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;
    //[SerializeField] Sprite halfHeart;

    private float _horizontal;
    private float _vertical;
    private int _currentHealth;
    private bool _isInvincible = false;

    private int _currentBombs = 0;
    private bool _canPlaceBomb = true;
    private Sequence _bombCooldownSequence;
    private List<Tween> _activeBombMonitors = new List<Tween>();

    public event System.Action<int> OnHealthChanged;
    public event System.Action OnPlayerDeath;
    public event System.Action<int, int> OnBombCountChanged; //current, max

    void Start()
    {
        _currentHealth = maxHealth;
        _currentBombs = 0;
        OnBombCountChanged?.Invoke(_currentBombs, PlayerUpgradeManager.Instance.GetBombCount);
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        _bombCooldownSequence?.Kill();
        foreach (var monitor in _activeBombMonitors)
        {
            monitor?.Kill();
        }
        _activeBombMonitors.Clear();
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

    public void OnPlaceBomb(InputAction.CallbackContext context)
    {
        if (context.performed && _canPlaceBomb && _currentBombs < PlayerUpgradeManager.Instance.GetBombCount)
        {
            TryPlaceBomb();
        }
    }

    private void TryPlaceBomb()
    {
        Vector2 bombPosition = GetGridPosition(transform.position);

        if (!IsPositionClear(bombPosition))
        {
            Debug.Log("Cannot place bomb - position occupied");
            return;
        }

        PlaceBomb(bombPosition);
    }

    private Vector2 GetGridPosition(Vector2 worldPosition)
    {
        return new Vector2(Mathf.Round(worldPosition.x) - 0.5f, Mathf.Round(worldPosition.y) - 0.5f);
    }

    private void PlaceBomb(Vector2 position)
    {
        GameObject bombObj = Instantiate(bombPrefab, position, Quaternion.identity);
        Bomb bomb = bombObj.GetComponent<Bomb>();
        bomb.UpdateBombStats(PlayerUpgradeManager.Instance.GetExplosionRange);
        
        bomb.OnExploded += (explodedBomb) => {
            _currentBombs--;
            OnBombCountChanged?.Invoke(_currentBombs, PlayerUpgradeManager.Instance.GetBombCount);
        };
    
        _currentBombs++;
        OnBombCountChanged?.Invoke(_currentBombs, PlayerUpgradeManager.Instance.GetBombCount);
    }

    private bool IsPositionClear(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.4f, obstacleLayerMask);
        return hit == null;
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
            Debug.Log("Picked up pickup");
            pickup.OnPickup(this);
        }
        else if (collision.TryGetComponent<IDamageDealer>(out var damager))
        {
            damager.ApplyDamage(this);
        }
    }
}