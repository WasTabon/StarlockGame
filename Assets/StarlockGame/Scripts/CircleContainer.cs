using UnityEngine;
using System.Collections.Generic;

public class CircleContainer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float radius = 2f;
    [SerializeField] private int maxShapesInside = 10;
    [SerializeField] private int colliderSegments = 32;

    [Header("References")]
    [SerializeField] private SpriteRenderer circleVisual;
    [SerializeField] private EdgeCollider2D boundaryCollider;

    [Header("Runtime Info")]
    [SerializeField] private int currentShapesInside = 0;

    private List<GameObject> shapesInside = new List<GameObject>();
    private float spriteBaseSize = 4f;

    public float Radius => radius;
    public int MaxShapesInside => maxShapesInside;
    public int CurrentShapesInside => currentShapesInside;
    public bool IsFull => currentShapesInside >= maxShapesInside;

    public System.Action OnContainerFull;
    public System.Action<int> OnShapeCountChanged;

    private void Awake()
    {
        if (boundaryCollider == null)
        {
            boundaryCollider = GetComponentInChildren<EdgeCollider2D>();
        }
        
        if (circleVisual != null && circleVisual.sprite != null)
        {
            spriteBaseSize = circleVisual.sprite.bounds.size.x;
        }
    }

    private void Start()
    {
        GenerateBoundaryCollider();
        UpdateVisualSize();
    }

    public void SetRadius(float newRadius)
    {
        radius = newRadius;
        GenerateBoundaryCollider();
        UpdateVisualSize();
    }

    public void SetMaxShapes(int max)
    {
        maxShapesInside = max;
    }

    public bool CanAddShape()
    {
        return currentShapesInside < maxShapesInside;
    }

    public void AddShapeInside(GameObject shape)
    {
        if (!shapesInside.Contains(shape))
        {
            shapesInside.Add(shape);
            currentShapesInside = shapesInside.Count;
            OnShapeCountChanged?.Invoke(currentShapesInside);

            if (IsFull)
            {
                OnContainerFull?.Invoke();
            }
        }
    }

    public void RemoveShapeInside(GameObject shape)
    {
        if (shapesInside.Contains(shape))
        {
            shapesInside.Remove(shape);
            currentShapesInside = shapesInside.Count;
            OnShapeCountChanged?.Invoke(currentShapesInside);
        }
    }

    public List<GameObject> GetShapesInside()
    {
        return new List<GameObject>(shapesInside);
    }

    public void ClearAllShapes()
    {
        shapesInside.Clear();
        currentShapesInside = 0;
        OnShapeCountChanged?.Invoke(currentShapesInside);
    }

    public Vector2 GetRandomPointInside()
    {
        float randomRadius = Random.Range(0f, radius * 0.8f);
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        
        Vector2 localPoint = new Vector2(
            Mathf.Cos(randomAngle) * randomRadius,
            Mathf.Sin(randomAngle) * randomRadius
        );

        return (Vector2)transform.position + localPoint;
    }

    private void GenerateBoundaryCollider()
    {
        if (boundaryCollider == null)
        {
            Debug.LogWarning("CircleContainer: No EdgeCollider2D assigned!");
            return;
        }

        Vector2[] points = new Vector2[colliderSegments + 1];
        
        for (int i = 0; i <= colliderSegments; i++)
        {
            float angle = (i / (float)colliderSegments) * 360f * Mathf.Deg2Rad;
            points[i] = new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            );
        }

        boundaryCollider.points = points;
    }

    private void UpdateVisualSize()
    {
        if (circleVisual == null) return;

        if (circleVisual.sprite != null)
        {
            spriteBaseSize = circleVisual.sprite.bounds.size.x;
        }

        float diameter = radius * 2f;
        float scale = diameter / spriteBaseSize;
        circleVisual.transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        DrawCircleGizmo(transform.position, radius, 32);
    }

    private void DrawCircleGizmo(Vector3 center, float r, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(r, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0f);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
