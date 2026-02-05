using UnityEngine;

public class RotationController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private RotationPreset currentPreset;
    [SerializeField] private bool rotateClockwise = true;
    
    [Header("Runtime")]
    [SerializeField] private bool isRotating = true;

    private float currentSpeed;

    private void Start()
    {
        ApplyPreset();
    }

    private void Update()
    {
        if (!isRotating) return;
        if (currentSpeed == 0f) return;

        float direction = rotateClockwise ? -1f : 1f;
        float rotation = currentSpeed * direction * Time.deltaTime;
        transform.Rotate(0f, 0f, rotation);
    }

    public void SetPreset(RotationPreset preset)
    {
        currentPreset = preset;
        ApplyPreset();
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

    public void SetRotating(bool rotating)
    {
        isRotating = rotating;
    }

    public void SetDirection(bool clockwise)
    {
        rotateClockwise = clockwise;
    }

    public void Stop()
    {
        isRotating = false;
    }

    public void Resume()
    {
        isRotating = true;
    }

    private void ApplyPreset()
    {
        if (currentPreset != null)
        {
            currentSpeed = currentPreset.RotationSpeed;
        }
        else
        {
            Debug.LogWarning("RotationController: No preset assigned!");
            currentSpeed = 60f;
        }
    }

    public float CurrentSpeed => currentSpeed;
    public bool IsRotating => isRotating;
    public RotationPreset CurrentPreset => currentPreset;
}
