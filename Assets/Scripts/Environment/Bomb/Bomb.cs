using System.Collections;
using Environment.Bomb;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float defaultExplosionDelay = 3f;
    [SerializeField] private int defaultExplosionRadius = 1;
    [SerializeField] private int defaultDamage = 1;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject explosionPrefab;
    
    private float _explosionDelay;
    private int _explosionRadius;
    private int _damage;
    
    private void Start()
    {
        Initialize(defaultExplosionDelay, defaultExplosionRadius, defaultDamage);
    }
    
    private void Initialize(float delay, int radius, int damage)
    {
        _explosionDelay = delay;
        _explosionRadius = radius;
        _damage = damage;
        StartCoroutine(ExplodeAfterDelay());
    }
    
    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(_explosionDelay);
        Explode();
    }
    
    private void Explode()
    {
        CreateExplosion(transform.position, ExplosionType.Center);
        CreateExplosionLine(Vector2.down);
        CreateExplosionLine(Vector2.left);
        CreateExplosionLine(Vector2.right);
        CreateExplosionLine(Vector2.up);
        Destroy(gameObject);
    }

    private void CreateExplosionLine(Vector2 direction)
    {
        for (int i = 1; i <= _explosionRadius; i++)
        {
            Vector2 explosionPos = (Vector2)transform.position + direction * i;
            
            if (CheckForObstacle(explosionPos))
            {
                CreateExplosion(explosionPos, ExplosionType.End, direction);
                break;
            }
            

            ExplosionType type = GetExplosionTypeForPosition(i, direction);
            CreateExplosion(explosionPos, type, direction);
        }
    }
    
    private void CreateExplosion(Vector2 position, ExplosionType type, Vector2 direction = default)
    {
        GameObject explosionObj = Instantiate(explosionPrefab, position, Quaternion.identity);
        Explosion explosion = explosionObj.GetComponent<Explosion>();
        
        explosion.Initialize(type, direction, _damage);
    }

    private bool CheckForObstacle(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.3f);
        
        if (hit != null)
        {
            return hit.CompareTag("Indestructible") || hit.CompareTag("Destructible");
        }
        
        return false;
    }
}
