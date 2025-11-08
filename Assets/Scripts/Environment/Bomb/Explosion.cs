using System;
using Environment.Bomb;
using UnityEngine;
using UpgradeSystem.Interfaces;

public class Explosion : MonoBehaviour, IDamageDealer
{
    [Header("Sprites")]
    [SerializeField] private Sprite _centerSprite;
    [SerializeField] private Sprite _sideSprite;
    [SerializeField] private Sprite _endSprite;
    
    [Header("Settings")]
    [SerializeField] private float timeToLive = 1f;

    private int _explosionDamage = 0;
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(ExplosionType type, Vector2 direction, int damage)
    {
        SetSprite(type);
        SetRotation(type, direction);
        _explosionDamage = damage;
        Destroy(gameObject, timeToLive);
    }
    
    private void SetSprite(ExplosionType explosionType)
    {
        _spriteRenderer.sprite = explosionType switch
        {
            ExplosionType.Center => _centerSprite,
            ExplosionType.Side => _sideSprite,
            ExplosionType.End => _endSprite,
            _ => _spriteRenderer.sprite
        };
    }
    
    private void SetRotation(ExplosionType explosionType, Vector2 direction)
    {
        if (explosionType is ExplosionType.End or ExplosionType.Side)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }
    public void ApplyDamage(IDamageable damageable)
    {
        damageable.TakeDamage(_explosionDamage);
    }
}