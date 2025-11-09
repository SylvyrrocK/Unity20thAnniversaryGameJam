using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UpgradeSystem.Interfaces;

public class GameManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnCheckRadius = 0.5f;
    [SerializeField] private LayerMask obstacleLayerMask;
    
    [Header("Wave Settings")]
    [SerializeField] private int startMaxEnemies = 5;
    [SerializeField] private int enemiesIncreasePerWave = 1;
    [SerializeField] private float waveDuration = 30f;
    [SerializeField] private float portalSpawnDistance = 4f;
    
    [Header("Portal Settings")]
    [SerializeField] private int maxPortals = 2;
    [SerializeField] private float portalSpawnInterval = 10f;
    [SerializeField] private float minDistanceFromPlayer = 3f;
    [SerializeField] private float maxDistanceFromPlayer = 6f;
    
    [Header("Game Area Settings")]
    [SerializeField] private Vector2 gameAreaMin = new Vector2(-7.5f, -6.5f);
    [SerializeField] private Vector2 gameAreaMax = new Vector2(7.5f, 6.5f);
    
    public static GameManager Instance { get; private set; }
    
    private Transform player;
    private List<GameObject> activePortals = new List<GameObject>();
    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentMaxEnemies;
    private int currentWave = 1;
    private float waveTimer;
    private float portalSpawnTimer;
    private int totalEnemiesKilled = 0;
    
    public event System.Action<int> OnWaveChanged;
    public event System.Action<int> OnEnemiesCountChanged;
    public event System.Action<int> OnTotalKillsChanged;
    
    public int GetCurrentWave() => currentWave;
    public int GetCurrentEnemies() => activeEnemies.Count;
    public int GetMaxEnemies() => currentMaxEnemies;
    public int GetTotalKills() => totalEnemiesKilled;
    public float GetWaveProgress() => 1f - (waveTimer / waveDuration);
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentMaxEnemies = startMaxEnemies;
        waveTimer = waveDuration;
        portalSpawnTimer = portalSpawnInterval;

        SpawnInitialEnemies();
    }
    
    void Update()
    {
        if (player is null) return;
        
        UpdateWaveSystem();
        UpdatePortalSystem();
    }

    private void UpdateWaveSystem()
    {
        waveTimer -= Time.deltaTime;
        
        if (waveTimer <= 0)
        {
            NextWave();
        }
    }
    
    private void UpdatePortalSystem()
    {
        portalSpawnTimer -= Time.deltaTime;
        
        if (portalSpawnTimer <= 0 && activePortals.Count < maxPortals)
        {
            TrySpawnPortal();
            portalSpawnTimer = portalSpawnInterval;
        }
        
        activePortals.RemoveAll(portal => portal == null);
    }
    
    public void SpawnEnemyAtPosition(Vector2 position)
    {
        if (enemyPrefabs.Length == 0) return;
    
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        activeEnemies.Add(enemy);
    
        Debug.Log($"✅ Enemy spawned from portal at: {position}");
        OnEnemiesCountChanged?.Invoke(activeEnemies.Count);
    }
    
    private void NextWave()
    {
        currentWave++;
        currentMaxEnemies = startMaxEnemies + (currentWave - 1) * enemiesIncreasePerWave;
        waveTimer = waveDuration;
        
        OnWaveChanged?.Invoke(currentWave);
        Debug.Log($"Wave {currentWave} started! Max enemies: {currentMaxEnemies}");
        
        if (currentWave % 2 == 0 && maxPortals < 4)
        {
            maxPortals++;
        }
    }
    
    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < startMaxEnemies; i++)
        {
            Vector2 spawnPos = FindFreeSpawnPosition();
            if (spawnPos != Vector2.negativeInfinity)
            {
                SpawnEnemy(spawnPos);
            }
        }
    }
    
    private void TrySpawnPortal()
    {
        Vector2 spawnPos = FindPortalSpawnPosition();
        if (spawnPos != Vector2.negativeInfinity)
        {
            GameObject portal = Instantiate(portalPrefab, spawnPos, Quaternion.identity);
            activePortals.Add(portal);
        }
    }
    
    private void TrySpawnEnemyFromPortal()
    {
        if (activePortals.Count == 0) return;
        
        // Выбираем случайный портал
        GameObject portal = activePortals[Random.Range(0, activePortals.Count)];
        if (portal != null)
        {
            SpawnEnemy(portal.transform.position);
        }
    }
    
    private Vector2 FindPortalSpawnPosition()
    {
        for (int i = 0; i < 20; i++) // 20 попыток найти позицию
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(gameAreaMin.x, gameAreaMax.x),
                Random.Range(gameAreaMin.y, gameAreaMax.y)
            );
            
            Vector2 gridPos = new Vector2(Mathf.Round(spawnPos.x), Mathf.Round(spawnPos.y));
            
            if (IsPositionFree(gridPos) && 
                !IsTooCloseToOtherPortal(gridPos) &&
                IsInsideGameArea(gridPos))
            {
                return gridPos;
            }
        }
        
        return Vector2.negativeInfinity;
    }
    
    private Vector2 FindFreeSpawnPosition()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(gameAreaMin.x, gameAreaMax.x),
                Random.Range(gameAreaMin.y, gameAreaMax.y)
            );
            
            Vector2 gridPos = new Vector2(Mathf.Round(spawnPos.x), Mathf.Round(spawnPos.y));
            
            if (IsPositionFree(gridPos) && 
                Vector2.Distance(gridPos, player.position) > 3f &&
                IsInsideGameArea(gridPos))
            {
                return gridPos;
            }
        }
        
        return Vector2.negativeInfinity;
    }
    
    private bool IsInsideGameArea(Vector2 position)
    {
        return position.x >= gameAreaMin.x && position.x <= gameAreaMax.x &&
               position.y >= gameAreaMin.y && position.y <= gameAreaMax.y;
    }
    
    private void SpawnEnemy(Vector2 position)
    {
        if (enemyPrefabs.Length == 0) return;
        
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        activeEnemies.Add(enemy);
        
        OnEnemiesCountChanged?.Invoke(activeEnemies.Count);
    }
    
    private bool IsPositionFree(Vector2 position)
    {
        if (!IsInsideGameArea(position))
        {
            return false;
        }
        
        Collider2D hit = Physics2D.OverlapCircle(position, spawnCheckRadius, obstacleLayerMask);
        return hit is null;
    }
    
    bool IsTooCloseToOtherPortal(Vector2 position)
    {
        foreach (GameObject portal in activePortals)
        {
            if (portal is not null && Vector2.Distance(position, portal.transform.position) < 2f)
            {
                return true;
            }
        }
        return false;
    }
}