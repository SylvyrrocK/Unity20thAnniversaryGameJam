using UpgradeSystem.Interfaces;

public class MoveSpeedUpgrade : Pickup
{
    public override void Interact(PlayerController player)
    {
        PlayerUpgradeManager.Instance.AddSpeedBoost(0.25f);
        Destroy(this);
    }
}
