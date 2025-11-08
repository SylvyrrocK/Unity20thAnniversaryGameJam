using System;
using UnityEngine;
using UpgradeSystem.Interfaces;

public class BombCountUpgrade : MonoBehaviour, Pickup
{
    public void OnPickup(PlayerController player)
    {
        PlayerUpgradeManager.Instance.AddBombCount();
        Debug.Log("Upgraded bomb count to " + PlayerUpgradeManager.Instance.GetBombCount);
        Destroy(gameObject);
    }
}
