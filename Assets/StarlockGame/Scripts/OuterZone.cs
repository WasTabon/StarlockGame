using UnityEngine;
using System.Collections.Generic;

public class OuterZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float innerRadius = 2.2f;
    [SerializeField] private float outerRadius = 4f;
    [SerializeField] private int colliderSegments = 32;

    [Header("References")]
    [SerializeField] private EdgeCollider2D outerBoundaryCollider;

    [Header("Runtime Info")]
    [SerializeField] private int shapesOutsideCount = 0;

    private List<GameObject> shapesOutside = new List<GameObject>();

    public float InnerRadius => innerRadius;
    public float OuterRadius => outerRadius;
    public int ShapesOutsideCount => shapesOutside.Count;
    public bool IsEmpty => shapesOutside.Count == 0;

    public System.Action OnAllShapesCleared;
    public System.Action<int> OnShapeCountChanged;

    private void Awake()
    {
        if (outerBoundaryCollider == null)
        {
            outerBoundaryCollider = GetComponentInChildren<EdgeCollider2D>();
        }
    }

    private void Start()
    {
        GenerateOuterBoundary();
    }

    public void SetRadii(float inner, float outer)
    {
        innerRadius = inner;
        outerRadius = outer;
        GenerateOuterBoundary();
    }

    public void AddShapeOutside(GameObject shape)
    {
        if (!shapesOutside.Contains(shape))
        {
            shapesOutside.Add(shape);
            shapesOutsideCount = shapesOutside.Count;
            OnShapeCountChanged?.Invoke(shapesOutsideCount);
        }
    }

    public void RemoveShapeOutside(GameObject shape)
    {
        if (shapesOutside.Contains(shape))
        {
            shapesOutside.Remove(shape);
            shapesOutsideCount = shapesOutside.Count;
            OnShapeCountChanged?.Invoke(shapesOutsideCount);

            if (IsEmpty)
            {
                OnAllShapesCleared?.Invoke();
            }
        }
    }

    public List<GameObject> GetShapesOutside()
    {
        return new List<GameObject>(shapesOutside);
    }

    public void ClearAllShapes()
    {
        shapesOutside.Clear();
        shapesOutsideCount = 0;
        OnShapeCountChanged?.Invoke(shapesOutsideCount);
    }

    public Vector2 GetRandomPointInZone()
    {
        float randomRadius = Random.Range(innerRadius, outerRadius);
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector2 localPoint = new Vector2(
            Mathf.Cos(randomAngle) * randomRadius,
            Mathf.Sin(randomAngle) * randomRadius
        );

        return (Vector2)transform.position + localPoint;
    }

    private void GenerateOuterBoundary()
    {
        if (outerBoundaryCollider == null)
        {
            Debug.LogWarning("OuterZone: No EdgeCollider2D assigned!");
            return;
        }

        Vector2[] points = new Vector2[colliderSegments + 1];

        for (int i = 0; i <= colliderSegments; i++)
        {
            float angle = (i / (float)colliderSegments) * 360f * Mathf.Deg2Rad;
            points[i] = new Vector2(
                Mathf.Cos(angle) * outerRadius,
                Mathf.Sin(angle) * outerRadius
            );
        }

        outerBoundaryCollider.points = points;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        DrawCircleGizmo(transform.position, innerRadius, 32);
        
        Gizmos.color = Color.red;
        DrawCircleGizmo(transform.position, outerRadius, 32);
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
