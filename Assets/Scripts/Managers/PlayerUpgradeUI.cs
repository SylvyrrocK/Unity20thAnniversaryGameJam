using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerUpgradeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text bombCountText;
    [SerializeField] private TMP_Text explosionRangeText;
    [SerializeField] private TMP_Text speedBoostText;

    private void OnEnable()
    {
        if (PlayerUpgradeManager.Instance != null)
            PlayerUpgradeManager.Instance.OnPlayerUpgrade += UpdateUI;

        RefreshDisplay();
    }

    private void OnDisable()
    {
        if (PlayerUpgradeManager.Instance != null)
            PlayerUpgradeManager.Instance.OnPlayerUpgrade -= UpdateUI;
    }

    private void UpdateUI(PlayerUpgradeManager.StatType stat, int _)
    {
        RefreshDisplay();

        switch (stat)
        {
            case PlayerUpgradeManager.StatType.BombCount:
                AnimateTextRight(bombCountText);
                break;
            case PlayerUpgradeManager.StatType.ExplosionRange:
                AnimateTextRight(explosionRangeText);
                break;
            case PlayerUpgradeManager.StatType.SpeedBoost:
                AnimateTextRight(speedBoostText);
                break;
        }
    }

    private void RefreshDisplay()
    {
        if (PlayerUpgradeManager.Instance == null) return;

        bombCountText.text = $"{PlayerUpgradeManager.Instance.GetBombCount:00}";
        explosionRangeText.text = $"{PlayerUpgradeManager.Instance.GetExplosionRange:00}";
        speedBoostText.text = $"{PlayerUpgradeManager.Instance.GetSpeedBost:00}";
    }

    private void AnimateTextRight(TMP_Text text)
    {
        // Kill ongoing tweens
        text.transform.DOKill();

        // Store original position
        Vector3 originalPos = text.transform.localPosition;

        // Move slightly to the right and back
        text.transform
            .DOLocalMoveX(originalPos.x + 20f, 0.15f) // adjust 20f for distance
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
                text.transform.DOLocalMoveX(originalPos.x, 0.15f).SetEase(Ease.InOutQuad)
            );

        // Optional subtle fade flash
        text.DOFade(1f, 0.1f).From(0.5f).SetEase(Ease.OutQuad);
    }
}