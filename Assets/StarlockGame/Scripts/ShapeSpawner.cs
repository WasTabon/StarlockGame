using UnityEngine;
using System.Collections.Generic;

public class ShapeSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShapeFactory shapeFactory;
    [SerializeField] private OuterZone outerZone;
    [SerializeField] private OuterZonePhysics outerZonePhysics;

    [Header("Spawn Settings")]
    [SerializeField] private int pairsToSpawn = 5;
    [SerializeField] private float minDistanceBetweenShapes = 0.5f;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Debug")]
    [SerializeField] private List<Shape> spawnedShapes = new List<Shape>();

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnInitialShapes();
        }
    }

    public void SpawnInitialShapes()
    {
        ClearAllShapes();

        for (int i = 0; i < pairsToSpawn; i++)
        {
            SpawnRandomPair();
        }
    }

    public void SpawnRandomPair()
    {
        ShapeType type = (ShapeType)Random.Range(0, System.Enum.GetValues(typeof(ShapeType)).Length);
        ShapeColor color = (ShapeColor)Random.Range(0, System.Enum.GetValues(typeof(ShapeColor)).Length);

        Vector3 pos1 = GetRandomSpawnPosition();
        Vector3 pos2 = GetRandomSpawnPosition();

        int attempts = 0;
        while (Vector3.Distance(pos1, pos2) < minDistanceBetweenShapes && attempts < 20)
        {
            pos2 = GetRandomSpawnPosition();
            attempts++;
        }

        Shape[] pair = shapeFactory.CreateMatchingPair(type, color, pos1, pos2);
        
        foreach (Shape shape in pair)
        {
            RegisterShape(shape);
        }
    }

    public Shape SpawnSingleShape(ShapeType type, ShapeColor color)
    {
        Vector3 position = GetRandomSpawnPosition();
        Shape shape = shapeFactory.CreateShape(type, color, position);
        RegisterShape(shape);
        return shape;
    }

    public Shape SpawnRandomShape()
    {
        Vector3 position = GetRandomSpawnPosition();
        Shape shape = shapeFactory.CreateRandomShape(position);
        RegisterShape(shape);
        return shape;
    }

    private void RegisterShape(Shape shape)
    {
        if (shape == null) return;

        spawnedShapes.Add(shape);

        if (outerZonePhysics != null)
        {
            outerZonePhysics.RegisterShape(shape);
        }

        if (outerZone != null)
        {
            outerZone.AddShapeOutside(shape.gameObject);
        }

        shape.OnMatched += OnShapeMatched;
        shape.OnStateChanged += OnShapeStateChanged;
    }

    private void OnShapeMatched(Shape shape)
    {
        spawnedShapes.Remove(shape);
        
        if (outerZonePhysics != null)
        {
            outerZonePhysics.UnregisterShape(shape);
        }

        if (outerZone != null)
        {
            outerZone.RemoveShapeOutside(shape.gameObject);
        }
    }

    private void OnShapeStateChanged(Shape shape)
    {
        if (shape.State == ShapeState.Inside || shape.State == ShapeState.MovingInside)
        {
            if (outerZonePhysics != null)
            {
                outerZonePhysics.UnregisterShape(shape);
            }

            if (outerZone != null)
            {
                outerZone.RemoveShapeOutside(shape.gameObject);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (outerZone == null)
        {
            Debug.LogWarning("ShapeSpawner: OuterZone not assigned!");
            return Vector3.zero;
        }

        float innerRadius = outerZone.InnerRadius;
        float outerRadius = outerZone.OuterRadius;

        float midRadius = (innerRadius + outerRadius) * 0.5f;
        float radiusVariance = (outerRadius - innerRadius) * 0.3f;

        float spawnRadius = midRadius + Random.Range(-radiusVariance, radiusVariance);
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        return new Vector3(
            Mathf.Cos(angle) * spawnRadius,
            Mathf.Sin(angle) * spawnRadius,
            0f
        );
    }

    public void ClearAllShapes()
    {
        foreach (Shape shape in spawnedShapes)
        {
            if (shape != null)
            {
                shape.OnMatched -= OnShapeMatched;
                shape.OnStateChanged -= OnShapeStateChanged;
                Destroy(shape.gameObject);
            }
        }

        spawnedShapes.Clear();

        if (outerZonePhysics != null)
        {
            outerZonePhysics.ClearAllShapes();
        }

        if (outerZone != null)
        {
            outerZone.ClearAllShapes();
        }
    }

    public int GetSpawnedCount()
    {
        spawnedShapes.RemoveAll(s => s == null);
        return spawnedShapes.Count;
    }

    public List<Shape> GetSpawnedShapes()
    {
        spawnedShapes.RemoveAll(s => s == null);
        return new List<Shape>(spawnedShapes);
    }

    public void SetPairsToSpawn(int pairs)
    {
        pairsToSpawn = pairs;
    }

    public void SetSpawnOnStart(bool spawn)
    {
        spawnOnStart = spawn;
    }
}
