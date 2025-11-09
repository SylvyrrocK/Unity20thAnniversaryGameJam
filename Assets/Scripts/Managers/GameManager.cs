using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UpgradeSystem.Interfaces;

public class GameManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private LayerMask obstacleLayerMask;
    
    [Header("Wave Settings")]
    [SerializeField] private int startMaxEnemies = 6;
    [SerializeField] private int enemiesIncreasePerWave = 1;
    [SerializeField] private float waveDuration = 30f;
    [SerializeField] private float portalSpawnDistance = 4f;
    
    [Header("Portal Settings")]
    [SerializeField] private int maxPortals = 2;
    [SerializeField] private float portalSpawnInterval = 10f;
    [SerializeField] private float minDistanceFromPlayer = 3f;
    [SerializeField] private float maxDistanceFromPlayer = 6f;
    
    [Header("Game Area Settings")]
    [SerializeField] private int gridWidth = 15;
    [SerializeField] private int gridHeight = 13;
    
    private int gameAreaMinX, gameAreaMaxX, gameAreaMinY, gameAreaMaxY;
    
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
        gameAreaMinX = -gridWidth / 2;
        gameAreaMinY = -gridHeight / 2;
        gameAreaMaxX = gridWidth / 2;
        gameAreaMaxY = gridHeight / 2;
        
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
        UpdateEnemyList();
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
    
    private void UpdateEnemyList()
    {
        // Удаляем уничтоженных врагов из списка
        int removedCount = activeEnemies.RemoveAll(enemy => enemy == null);
        if (removedCount > 0)
        {
            Debug.Log($"Removed {removedCount} dead enemies. Active: {activeEnemies.Count}");
            OnEnemiesCountChanged?.Invoke(activeEnemies.Count);
        }
    }
    
    public void OnEnemyDied(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            totalEnemiesKilled++;
            OnEnemiesCountChanged?.Invoke(activeEnemies.Count);
            OnTotalKillsChanged?.Invoke(totalEnemiesKilled);
            Debug.Log($"💀 Enemy died. Total: {activeEnemies.Count}/{currentMaxEnemies}, Kills: {totalEnemiesKilled}");
        }
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
                Random.Range(gameAreaMinX, gameAreaMaxX + 1),
                Random.Range(gameAreaMinY, gameAreaMaxY + 1)
            );
            
            if (IsPositionFree(spawnPos) && 
                !IsTooCloseToOtherPortal(spawnPos) &&
                Vector2.Distance(spawnPos, player.position) >= minDistanceFromPlayer)
            {
                return spawnPos;
            }
        }
        
        return Vector2.negativeInfinity;
    }
    
    private Vector2 FindFreeSpawnPosition()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(gameAreaMinX, gameAreaMaxX + 1),
                Random.Range(gameAreaMinY, gameAreaMaxY + 1)
            );
        
            if (IsPositionFree(spawnPos) && 
                Vector2.Distance(spawnPos, player.position) > 3f)
            {
                return spawnPos;
            }
            else
            {
                Debug.Log("Attempt" + i + "busy - " + spawnPos);
            }
        }
        
        return Vector2.negativeInfinity;
    }
    
    private bool IsInsideGameArea(Vector2 position)
    {
        return position.x >= gameAreaMinX && position.x <= gameAreaMaxX &&
               position.y >= gameAreaMinY && position.y <= gameAreaMaxY;
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
        
        Collider2D hit = Physics2D.OverlapPoint(position, obstacleLayerMask);
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