using UnityEngine;

public class GameplayScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotationPivot;
    [SerializeField] private CircleContainer circleContainer;
    [SerializeField] private OuterZone outerZone;
    [SerializeField] private Camera mainCamera;

    [Header("Settings")]
    [SerializeField] [Range(0f, 0.5f)] private float horizontalPadding = 0.15f;
    [SerializeField] private float baseCircleRadius = 2f;
    [SerializeField] private float baseOuterRadius = 4f;
    [SerializeField] private float baseInnerRadius = 2.2f;
    [SerializeField] private float pivotZPosition = 0f;

    [Header("Debug")]
    [SerializeField] private float currentScale = 1f;
    [SerializeField] private float availableWidth;

    private float lastScreenWidth;
    private float lastScreenHeight;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (rotationPivot != null)
        {
            pivotZPosition = rotationPivot.position.z;
        }

        ApplyScaling();
    }

    private void Update()
    {
        if (Mathf.Abs(Screen.width - lastScreenWidth) > 1f || Mathf.Abs(Screen.height - lastScreenHeight) > 1f)
        {
            ApplyScaling();
        }
    }

    public void ApplyScaling()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        if (mainCamera == null)
        {
            Debug.LogWarning("GameplayScaler: No camera assigned!");
            return;
        }

        float worldHeight = mainCamera.orthographicSize * 2f;
        float worldWidth = worldHeight * mainCamera.aspect;

        float usableWidth = worldWidth * (1f - horizontalPadding * 2f);

        float neededWidth = baseOuterRadius * 2f;
        currentScale = usableWidth / neededWidth;

        availableWidth = usableWidth;

        ApplyToRotationPivot();
        ApplyToCircleContainer();
        ApplyToOuterZone();
        CenterPivot();
    }

    private void ApplyToRotationPivot()
    {
        if (rotationPivot == null)
        {
            Debug.LogWarning("GameplayScaler: RotationPivot not assigned!");
            return;
        }

        rotationPivot.localScale = Vector3.one * currentScale;
    }

    private void ApplyToCircleContainer()
    {
        if (circleContainer == null) return;

        circleContainer.SetRadius(baseCircleRadius);
    }

    private void ApplyToOuterZone()
    {
        if (outerZone == null) return;

        outerZone.SetRadii(baseInnerRadius, baseOuterRadius);
    }

    private void CenterPivot()
    {
        if (rotationPivot == null) return;

        rotationPivot.position = new Vector3(0f, 0f, pivotZPosition);
    }

    public float GetCurrentScale()
    {
        return currentScale;
    }

    public float GetScaledRadius(float baseRadius)
    {
        return baseRadius * currentScale;
    }

    public void SetHorizontalPadding(float padding)
    {
        horizontalPadding = Mathf.Clamp(padding, 0f, 0.5f);
        ApplyScaling();
    }

    public void SetBaseRadii(float circleRadius, float innerRadius, float outerRadius)
    {
        baseCircleRadius = circleRadius;
        baseInnerRadius = innerRadius;
        baseOuterRadius = outerRadius;
        ApplyScaling();
    }

    public void SetPivotZPosition(float z)
    {
        pivotZPosition = z;
        CenterPivot();
    }

    private void OnDrawGizmosSelected()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        float worldHeight = mainCamera.orthographicSize * 2f;
        float worldWidth = worldHeight * mainCamera.aspect;

        Gizmos.color = Color.red;
        float leftEdge = -worldWidth / 2f;
        float rightEdge = worldWidth / 2f;
        float topEdge = worldHeight / 2f;
        float bottomEdge = -worldHeight / 2f;

        Gizmos.DrawLine(new Vector3(leftEdge, bottomEdge, 0), new Vector3(leftEdge, topEdge, 0));
        Gizmos.DrawLine(new Vector3(rightEdge, bottomEdge, 0), new Vector3(rightEdge, topEdge, 0));

        Gizmos.color = Color.green;
        float paddingWorld = worldWidth * horizontalPadding;
        float leftPadded = leftEdge + paddingWorld;
        float rightPadded = rightEdge - paddingWorld;

        Gizmos.DrawLine(new Vector3(leftPadded, bottomEdge, 0), new Vector3(leftPadded, topEdge, 0));
        Gizmos.DrawLine(new Vector3(rightPadded, bottomEdge, 0), new Vector3(rightPadded, topEdge, 0));
    }
}
