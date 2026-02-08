using UnityEngine;
using DG.Tweening;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Camera mainCamera;

    [Header("Settings")]
    [SerializeField] private float defaultDuration = 0.3f;
    [SerializeField] private float defaultStrength = 0.2f;
    [SerializeField] private int defaultVibrato = 20;
    [SerializeField] private float defaultRandomness = 90f;

    private Vector3 originalPosition;
    private Tween shakeTween;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            originalPosition = mainCamera.transform.position;
        }
    }

    public void Shake()
    {
        Shake(defaultDuration, defaultStrength, defaultVibrato, defaultRandomness);
    }

    public void ShakeLight()
    {
        Shake(0.15f, 0.1f, 10, 90f);
    }

    public void ShakeHeavy()
    {
        Shake(0.5f, 0.4f, 30, 90f);
    }

    public void Shake(float duration, float strength, int vibrato, float randomness)
    {
        if (mainCamera == null) return;

        shakeTween?.Kill();
        mainCamera.transform.position = originalPosition;

        shakeTween = mainCamera.transform.DOShakePosition(duration, strength, vibrato, randomness)
            .OnComplete(() =>
            {
                mainCamera.transform.position = originalPosition;
            });
    }

    public void StopShake()
    {
        shakeTween?.Kill();
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalPosition;
        }
    }

    private void OnDestroy()
    {
        shakeTween?.Kill();
    }
}
