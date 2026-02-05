using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CircleContainer circleContainer;
    [SerializeField] private OuterZonePhysics outerZonePhysics;

    [Header("Settings")]
    [SerializeField] private LayerMask shapeLayerMask = -1;
    [SerializeField] private float tapRadius = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    public System.Action<Shape> OnShapeTapped;

    private bool inputEnabled = true;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (!inputEnabled) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTap(touch.position);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            HandleTap(Input.mousePosition);
        }
    }

    private void HandleTap(Vector2 screenPosition)
    {
        if (IsPointerOverUI())
        {
            if (showDebugInfo) Debug.Log("Tap blocked by UI");
            return;
        }

        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        Shape tappedShape = FindShapeAtPosition(worldPosition);

        if (tappedShape != null)
        {
            if (showDebugInfo) Debug.Log($"Tapped shape: {tappedShape.Type} {tappedShape.Color}");
            ProcessShapeTap(tappedShape);
        }
        else
        {
            if (showDebugInfo) Debug.Log("No shape at tap position");
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        return EventSystem.current.IsPointerOverGameObject();
    }

    private Shape FindShapeAtPosition(Vector2 worldPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, tapRadius, shapeLayerMask);

        Shape closestShape = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D col in colliders)
        {
            Shape shape = col.GetComponent<Shape>();
            if (shape != null && shape.State == ShapeState.Outside)
            {
                float distance = Vector2.Distance(worldPosition, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestShape = shape;
                }
            }
        }

        return closestShape;
    }

    private void ProcessShapeTap(Shape shape)
    {
        if (shape.State != ShapeState.Outside)
        {
            if (showDebugInfo) Debug.Log("Shape is not in Outside state, ignoring tap");
            return;
        }

        if (circleContainer != null && circleContainer.IsFull)
        {
            if (showDebugInfo) Debug.Log("Circle is full, cannot add more shapes");
            return;
        }

        OnShapeTapped?.Invoke(shape);

        SendShapeInside(shape);
    }

    private void SendShapeInside(Shape shape)
    {
        if (outerZonePhysics != null)
        {
            outerZonePhysics.UnregisterShape(shape);
        }

        Vector2 targetPosition = GetRandomPositionInsideCircle();

        shape.MoveInside(targetPosition, () =>
        {
            if (circleContainer != null)
            {
                circleContainer.AddShapeInside(shape.gameObject);
            }

            ApplyRandomBounceForce(shape);
        });
    }

    private Vector2 GetRandomPositionInsideCircle()
    {
        if (circleContainer == null)
        {
            return Vector2.zero;
        }

        float maxRadius = circleContainer.Radius * 0.6f;
        float randomRadius = Random.Range(0f, maxRadius);
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        return new Vector2(
            Mathf.Cos(randomAngle) * randomRadius,
            Mathf.Sin(randomAngle) * randomRadius
        );
    }

    private void ApplyRandomBounceForce(Shape shape)
    {
        Rigidbody2D rb = shape.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float forceMagnitude = Random.Range(2f, 4f);
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector2 force = new Vector2(
            Mathf.Cos(randomAngle) * forceMagnitude,
            Mathf.Sin(randomAngle) * forceMagnitude
        );

        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    public void SetTapRadius(float radius)
    {
        tapRadius = radius;
    }
}
