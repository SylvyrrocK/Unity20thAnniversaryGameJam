using UnityEngine;
using UpgradeSystem.Interfaces;

public class DestructibleWall : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 3;
    [SerializeField] GameObject destroyEffect;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Wall took " + damage + " damage. Remaining health: " + health);

        if (health <= 0)
        {
            DestroyWall();
        }
    }

    void DestroyWall()
    {
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
