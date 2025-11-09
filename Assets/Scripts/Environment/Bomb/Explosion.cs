using DG.Tweening;
using Environment.Bomb;
using UnityEngine;
using UpgradeSystem.Interfaces;

public class Explosion : MonoBehaviour, IDamageDealer
{
    [Header("Sprites")] [SerializeField] private Sprite _centerSprite;
    [SerializeField] private Sprite _sideSprite;
    [SerializeField] private Sprite _endSprite;

    [Header("Settings")] 
    [SerializeField] private float timeToLive = 1f;

    [Header("Gizmos Settings")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color circleColor = new Color(1, 0, 0, 0.3f);
    [SerializeField] private Color hitColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color damageableColor = new Color(1, 1, 0, 0.7f);
    [SerializeField] private bool showHitInfo = true;
    
    private int _explosionDamage = 0;

    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _circleCollider2D;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
    }

    public void Initialize(ExplosionType type, Vector2 direction, int damage)
    {
        SetSprite(type);
        SetRotation(type, direction);
        _explosionDamage = damage;

        ApplyDamageToSurroundings();

        _spriteRenderer.DOFade(0, timeToLive)
            .SetEase(Ease.InQuad);
        Destroy(gameObject, timeToLive);
    }

    private void ApplyDamageToSurroundings()
    {
        Vector2 circleCenter = GetCircleColliderCenter();
        float radius = GetCircleColliderRadius() * 0.8f;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(circleCenter, radius);
        
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                ApplyDamage(damageable);
            }
        }
    }
    
    private Vector2 GetCircleColliderCenter()
    {
        return (Vector2)transform.position + _circleCollider2D.offset;
    }

    private float GetCircleColliderRadius()
    {
        return _circleCollider2D.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
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
    
    private void OnDrawGizmos()
    {
        if (!showGizmos || _circleCollider2D == null) return;

        Vector2 circleCenter = GetCircleColliderCenter();
        float radius = GetCircleColliderRadius();
        
        Gizmos.color = circleColor;
        Gizmos.DrawWireSphere(circleCenter, radius);
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(circleCenter, radius);
        
        foreach (var hit in hits)
        {
            if (hit == null) continue;

            bool isDamageable = hit.TryGetComponent<IDamageable>(out _);
            Gizmos.color = isDamageable ? damageableColor : hitColor;

            DrawColliderGizmo(hit);

            Gizmos.color = isDamageable ? Color.yellow : Color.white;
            Gizmos.DrawLine(circleCenter, hit.bounds.center);

            if (showHitInfo)
            {
                #if UNITY_EDITOR
                string objectInfo = $"{hit.name}";
                if (isDamageable) objectInfo += "\n[DMG]";
                UnityEditor.Handles.Label(hit.bounds.center + Vector3.up * 0.2f, objectInfo);
                #endif
            }
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(circleCenter, 0.05f);
        Gizmos.DrawLine(transform.position, circleCenter);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos || _circleCollider2D == null) return;

        Vector2 circleCenter = GetCircleColliderCenter();
        float radius = GetCircleColliderRadius();

        Gizmos.color = new Color(1, 0, 0, 0.1f);
        Gizmos.DrawSphere(circleCenter, radius);

        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, 
            $"Radius: {radius:F2}\nOffset: {_circleCollider2D.offset}");
        #endif
    }

    private void DrawColliderGizmo(Collider2D collider)
    {
        if (collider is BoxCollider2D boxCollider)
        {
            Vector2 center = (Vector2)collider.transform.position + boxCollider.offset;
            Vector2 size = Vector2.Scale(boxCollider.size, collider.transform.lossyScale);
            Gizmos.DrawWireCube(center, size);
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            Vector2 center = (Vector2)collider.transform.position + circleCollider.offset;
            float radius = circleCollider.radius * Mathf.Max(collider.transform.lossyScale.x, collider.transform.lossyScale.y);
            Gizmos.DrawWireSphere(center, radius);
        }
        else if (collider is PolygonCollider2D)
        {
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}