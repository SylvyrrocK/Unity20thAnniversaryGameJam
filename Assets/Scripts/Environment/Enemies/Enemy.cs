using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UpgradeSystem.Interfaces;

public class Enemy : MonoBehaviour, IDamageable, IDamageDealer
{
    [SerializeField] protected int health = 1;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] private int graphIndex = 0;
    
    [SerializeField] private float nextWaypointDistance = 0.1f;
    
    [Header("Enemy Sprites")]
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;
    private Transform player;
    private Seeker seeker;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    private GridGraph gridGraph;
    
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        seeker = GetComponent<Seeker>();
        if (graphIndex >= 0 && graphIndex < AstarPath.active.graphs.Length)
        {
            seeker.graphMask = GraphMask.FromGraph(AstarPath.active.graphs[graphIndex]);
        }
        gridGraph = AstarPath.active.data.gridGraph;
        ConfigureGridForBomberman();
        InvokeRepeating("UpdatePath", 0f, 0.3f);
        
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void ConfigureGridForBomberman()
    {
        if (gridGraph != null)
        {
            // Размер ноды = размеру тайла
            gridGraph.nodeSize = 1f;
            
            // Обновляем граф
            AstarPath.active.Scan();
        }
    }

    void UpdatePath()
    {
        if (seeker.IsDone() && player is not null)
            seeker.StartPath(transform.position, player.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void Update()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;
        
        Vector2 targetPos = path.vectorPath[currentWaypoint];
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        
        Vector2 axisDirection = GetAxisAlignedDirection(direction);
        transform.position += (Vector3)axisDirection * (moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            currentWaypoint++;
        }
        
        UpdateSpriteDirection(direction);
    }

    Vector2 GetAxisAlignedDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return new Vector2(Mathf.Sign(direction.x), 0);
        else
            return new Vector2(0, Mathf.Sign(direction.y));
    }
    
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    public virtual void ApplyDamage(IDamageable target)
    {
        target.TakeDamage(damage);        
    }
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
    
    void UpdateSpriteDirection(Vector2 direction)
    {
        // Определяем основное направление по осям
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Движение по горизонтали
            if (direction.x > 0)
                spriteRenderer.sprite = rightSprite;
            else
                spriteRenderer.sprite = leftSprite;
        }
        else
        {
            // Движение по вертикали
            if (direction.y > 0)
                spriteRenderer.sprite = upSprite;
            else
                spriteRenderer.sprite = downSprite;
        }
    }
}
