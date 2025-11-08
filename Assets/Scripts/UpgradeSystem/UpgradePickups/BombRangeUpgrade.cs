using UnityEngine;
using UpgradeSystem.Interfaces;

public class BombRangeUpgrade : MonoBehaviour, Pickup
{
    public void OnPickup(PlayerController player)
    {
        PlayerUpgradeManager.Instance.AddRange();
        Debug.Log("Upgraded range to " + PlayerUpgradeManager.Instance.GetExplosionRange);
        Destroy(gameObject);
    }
}
