using UnityEngine;
using UpgradeSystem.Interfaces;

public class Explosion : MonoBehaviour, IDamageDealer
{
    [SerializeField] private float timeToLive = 1f;
    [SerializeField] private int damage = 1;
    
    public int GetDamage()
    {
        return damage;
    }
    public void ApplyDamage(IDamageable damageable)
    {
        damageable.TakeDamage(GetDamage());
    }
}