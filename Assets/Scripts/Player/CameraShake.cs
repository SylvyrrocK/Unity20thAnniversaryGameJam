using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineCamera cineCam;
    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        Instance = this;
        cineCam = GetComponent<CinemachineCamera>();
        noise = cineCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float intensity = 5f, float duration = 0.3f)
    {
        if (noise == null) return;

        // Set shake intensity
        noise.AmplitudeGain = intensity;

        // Smoothly tween back to 0
        DOVirtual.Float(intensity, 0, duration, v => noise.AmplitudeGain = v)
            .SetEase(Ease.OutQuad);
    }
}