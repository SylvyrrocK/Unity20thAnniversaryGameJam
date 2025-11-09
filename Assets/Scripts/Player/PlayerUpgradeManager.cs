using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
public enum StatType
    {
        BombCount,
        ExplosionRange,
        SpeedBoost
    }

    [SerializeField] AudioClip bombUpgradeClip;
    [SerializeField] AudioClip speedUpgradeClip;
    [SerializeField] AudioClip rangeUpgradeClip;

    private static PlayerUpgradeManager _instance;
    public static PlayerUpgradeManager Instance 
    {
        get 
        {
            if (!_instance) 
            {
                _instance = FindFirstObjectByType<PlayerUpgradeManager>();
            }
            return _instance;
        }
    }
    
    public event System.Action<StatType, int> OnPlayerUpgrade;
    
    private int _bombCount = 1;
    private int _explosionRange = 1;
    private float _speedBoost = 1;
    
    public int GetBombCount => _bombCount;
    public int GetExplosionRange => _explosionRange;
    public float GetSpeedBost => _speedBoost;

    public void AddBombCount()
    {
        _bombCount++;
        SoundFXManager.Instance.PlaySoundFXClip(bombUpgradeClip, transform, 1f);
        OnPlayerUpgrade?.Invoke(StatType.BombCount, _bombCount);
        
    }

    public void AddRange()
    {
        _explosionRange++;
        SoundFXManager.Instance.PlaySoundFXClip(rangeUpgradeClip, transform, 1f);
        OnPlayerUpgrade?.Invoke(StatType.ExplosionRange, _explosionRange);
    }

    public void AddSpeedBoost(float factor)
    {
        _speedBoost += factor;
        SoundFXManager.Instance.PlaySoundFXClip(speedUpgradeClip, transform, 1f);
        OnPlayerUpgrade?.Invoke(StatType.SpeedBoost, Mathf.RoundToInt(_speedBoost));
    } 
}
