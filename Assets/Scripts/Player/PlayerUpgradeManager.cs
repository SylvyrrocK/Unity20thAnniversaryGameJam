using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
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
    
    public event System.Action<int> OnPlayerUpgrade;
    
    private int _bombCount = 1;
    private int _explosionRange = 1;
    private float _speedBoost = 1;
    
    public int GetBombCount => _bombCount;
    public int GetExplosionRange => _explosionRange;
    public float GetSpeedBost => _speedBoost;

    public void AddBombCount()
    {
        _bombCount++; 
        OnPlayerUpgrade?.Invoke(_bombCount);
        
    }

    public void AddRange()
    {
        _explosionRange++;
        OnPlayerUpgrade?.Invoke(_explosionRange);
    }

    public void AddSpeedBoost(float factor)
    {
        _speedBoost += factor;
        OnPlayerUpgrade?.Invoke(_explosionRange);
    } 
}
