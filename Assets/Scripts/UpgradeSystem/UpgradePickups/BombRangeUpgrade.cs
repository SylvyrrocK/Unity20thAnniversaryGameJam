using UpgradeSystem.Interfaces;

public class BombRangeUpgrade : Pickup
{
    public override void Interact(PlayerController player)
    {
        PlayerUpgradeManager.Instance.AddRange();
        Destroy(this);
    }
}
