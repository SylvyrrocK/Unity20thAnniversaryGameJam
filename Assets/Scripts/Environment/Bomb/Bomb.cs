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
    [SerializeField] private AudioClip[] damageSoundClips;
    public event System.Action<Bomb> OnExploded;
    
    private BoxCollider2D bombCollider;
    private float _explosionDelay;
    private int _explosionRadius;
    private int _damage;
    private bool _isInitialized = false;

    private void Awake()
    {
        bombCollider = GetComponent<BoxCollider2D>();
        
        if (!_isInitialized)
        {
            _explosionDelay = defaultExplosionDelay;
            _explosionRadius = defaultExplosionRadius;
            _damage = defaultDamage;
            _isInitialized = true;
        }
    }
    
    private void Start()
    {
        bombCollider.isTrigger = true;
        StartCoroutine(ExplodeAfterDelay());
    }
    
    public void Initialize(float delay, int radius, int damage)
    {
        _explosionDelay = delay;
        _explosionRadius = radius;
        _damage = damage;
        _isInitialized = true;
    }
    
    public void UpdateBombStats(int radius)
    {
        _explosionRadius = radius;
    }
    
    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(_explosionDelay);
        Explode();
    }
    
    private void Explode()
    {
        OnExploded?.Invoke(this);

        SoundFXManager.Instance.PlayRandomSoundFXClip(damageSoundClips, transform, 1f);
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
            
            ExplosionType explosionType = GetExplosionTypeForPosition(i, direction);
            CreateExplosion(explosionPos, explosionType, direction);
        }
    }
    
    private ExplosionType GetExplosionTypeForPosition(int distance, Vector2 direction)
    {
        if (distance == _explosionRadius)
        {
            return ExplosionType.End;
        }
        
        return ExplosionType.Side;
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
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bombCollider.isTrigger = false;
        }
    }
}
