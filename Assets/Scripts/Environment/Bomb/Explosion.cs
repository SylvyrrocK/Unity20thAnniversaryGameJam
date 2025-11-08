using System;
using Environment.Bomb;
using UnityEngine;
using UpgradeSystem.Interfaces;

public class Explosion : MonoBehaviour, IDamageDealer
{
    [Header("Sprites")] [SerializeField] private Sprite _centerSprite;
    [SerializeField] private Sprite _sideSprite;
    [SerializeField] private Sprite _endSprite;

    [Header("Settings")] [SerializeField] private float timeToLive = 1f;

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

    public void ApplyDamage(IDamageable damageable)
    {
        damageable.TakeDamage(_explosionDamage);
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

    private void SetRotation(ExplosionType type, Vector2 direction)
    {
        float angle = 0f;
        
        switch (type)
        {
            case ExplosionType.Center:
                angle = 0f;
                break;
                
            case ExplosionType.Side:
                if (direction == Vector2.up || direction == Vector2.down)
                    angle = 0f;
                else
                    angle = 90f;
                break;
                
            case ExplosionType.End:
                if (direction == Vector2.up) angle = 0f;
                else if (direction == Vector2.down) angle = 180f;  
                else if (direction == Vector2.left) angle = 90f; 
                else if (direction == Vector2.right) angle = -90f;
                break;
        }
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}