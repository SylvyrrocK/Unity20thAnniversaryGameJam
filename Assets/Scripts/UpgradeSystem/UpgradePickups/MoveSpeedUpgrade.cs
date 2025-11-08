using UnityEngine;
using UpgradeSystem.Interfaces;

public class MoveSpeedUpgrade : MonoBehaviour, Pickup
{
    public void OnPickup(PlayerController player)
    {
        PlayerUpgradeManager.Instance.AddSpeedBoost(0.25f);
        Destroy(this);
    }
}
