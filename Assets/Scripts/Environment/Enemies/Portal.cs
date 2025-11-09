using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private float lifetime = 25f;
    [SerializeField] private float spawnInterval = 3f;
    
    private Animator animator;
    private float spawnTimer;
    private bool isClosing = false;

    void Awake()
    {
        animator = GetComponent<Animator>();    
    }

    void Start()
    {
        spawnTimer = spawnInterval;
        
        Invoke("ClosePortal", lifetime);
    }
    void Update()
    {
        if (isClosing) return;
        
        spawnTimer -= Time.deltaTime;
        
        if (spawnTimer <= 0)
        {
            if (CanSpawnEnemy())
            {
                SpawnEnemy();
            }
            spawnTimer = spawnInterval;
        }
    }

    bool CanSpawnEnemy()
    {
        return GameManager.Instance != null && 
               GameManager.Instance.GetCurrentEnemies() < GameManager.Instance.GetMaxEnemies();
    }

    public void ClosePortal()
    {
        if (isClosing) return;
        
        isClosing = true;
        StartCoroutine(ClosePortalRoutine());
    }
    
    void SpawnEnemy()
    {
        GameManager.Instance.SpawnEnemyAtPosition(transform.position);
        
    }
    
    private IEnumerator ClosePortalRoutine()
    {
        if (animator is not null)
        {
            animator.SetTrigger("Portal_Close");
        }
        yield return new WaitForEndOfFrame();
        float closeTime = GetCurrentAnimationLength();
        
        yield return new WaitForSeconds(closeTime);
        yield return new WaitForEndOfFrame();
        
        Destroy(gameObject);
    }
    
    private float GetCurrentAnimationLength()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.length;
        }
        
        return 1f;
    }
}