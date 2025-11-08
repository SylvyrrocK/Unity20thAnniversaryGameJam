using System;
using UnityEngine;
using UpgradeSystem.Interfaces;

public class BombCountUpgrade : Pickup
{
    public override void Interact(PlayerController player)
    {
        PlayerUpgradeManager.Instance.AddBombCount();
        Debug.Log("Upgraded bomb count to " + PlayerUpgradeManager.Instance.GetBombCount);
        Destroy(gameObject);
    }
}
