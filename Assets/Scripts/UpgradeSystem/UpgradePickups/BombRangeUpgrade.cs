using UnityEngine;
using UpgradeSystem.Interfaces;

public class BombRangeUpgrade : MonoBehaviour, Pickup
{
    public void OnPickup(PlayerController player)
    {
        PlayerUpgradeManager.Instance.AddRange();
        Destroy(this);
    }
}
