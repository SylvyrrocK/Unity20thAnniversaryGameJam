using UnityEngine;

namespace UpgradeSystem.Interfaces
{
    public abstract class Pickup: MonoBehaviour, IInteractable
    {
        [SerializeField] protected int pickupValue = 1;

        public abstract void Interact(PlayerController player);
    }
}