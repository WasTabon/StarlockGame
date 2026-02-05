using UnityEngine;
using System.Collections.Generic;

public class ShapeFactory : MonoBehaviour
{
    public static ShapeFactory Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float shapeSize = 0.4f;

    [Header("Physics Material")]
    [SerializeField] private float bounciness = 0.8f;
    [SerializeField] private float friction = 0.1f;

    [Header("References")]
    [SerializeField] private Transform shapesParent;

    private Dictionary<ShapeType, Sprite> shapeSprites = new Dictionary<ShapeType, Sprite>();
    private PhysicsMaterial2D bouncyMaterial;
    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Initialize();
    }

    public void Initialize()
    {
        if (isInitialized) return;

        CreatePhysicsMaterial();
        LoadOrGenerateSprites();
        isInitialized = true;
    }

    private void CreatePhysicsMaterial()
    {
        bouncyMaterial = new PhysicsMaterial2D("ShapeBouncyMaterial");
        bouncyMaterial.bounciness = bounciness;
        bouncyMaterial.friction = friction;
    }

    private void LoadOrGenerateSprites()
    {
        shapeSprites.Clear();

        foreach (ShapeType shapeType in System.Enum.GetValues(typeof(ShapeType)))
        {
            Sprite sprite = Resources.Load<Sprite>($"Shapes/{shapeType}");
            
            if (sprite == null)
            {
                Texture2D texture = ShapeVisualGenerator.GenerateShapeTexture(shapeType);
                sprite = ShapeVisualGenerator.CreateSprite(texture);
            }

            shapeSprites[shapeType] = sprite;
        }
    }

    public Shape CreateShape(ShapeType type, ShapeColor color, Vector3 position)
    {
        if (!isInitialized)
        {
            Initialize();
        }

        GameObject shapeObj = new GameObject($"Shape_{type}_{color}");
        
        if (shapesParent != null)
        {
            shapeObj.transform.SetParent(shapesParent);
        }
        
        shapeObj.transform.localPosition = position;
        shapeObj.transform.localScale = Vector3.one * shapeSize;
        shapeObj.transform.localRotation = Quaternion.identity;

        SpriteRenderer sr = shapeObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 10;

        Sprite sprite = shapeSprites.ContainsKey(type) ? shapeSprites[type] : null;
        sr.sprite = sprite;
        sr.color = color.ToColor();

        Rigidbody2D rb = shapeObj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sharedMaterial = bouncyMaterial;

        PolygonCollider2D col = shapeObj.AddComponent<PolygonCollider2D>();
        col.sharedMaterial = bouncyMaterial;

        Shape shape = shapeObj.AddComponent<Shape>();
        shape.Initialize(type, color, sprite);

        return shape;
    }

    public Shape CreateRandomShape(Vector3 position)
    {
        ShapeType randomType = (ShapeType)Random.Range(0, System.Enum.GetValues(typeof(ShapeType)).Length);
        ShapeColor randomColor = (ShapeColor)Random.Range(0, System.Enum.GetValues(typeof(ShapeColor)).Length);
        
        return CreateShape(randomType, randomColor, position);
    }

    public Shape[] CreateMatchingPair(ShapeType type, ShapeColor color, Vector3 position1, Vector3 position2)
    {
        Shape shape1 = CreateShape(type, color, position1);
        Shape shape2 = CreateShape(type, color, position2);
        
        return new Shape[] { shape1, shape2 };
    }

    public void SetShapesParent(Transform parent)
    {
        shapesParent = parent;
    }

    public void SetShapeSize(float size)
    {
        shapeSize = size;
    }

    public Sprite GetSpriteForType(ShapeType type)
    {
        if (!isInitialized)
        {
            Initialize();
        }

        return shapeSprites.ContainsKey(type) ? shapeSprites[type] : null;
    }

    public PhysicsMaterial2D GetBouncyMaterial()
    {
        if (bouncyMaterial == null)
        {
            CreatePhysicsMaterial();
        }
        return bouncyMaterial;
    }

    public void SetPhysicsSettings(float newBounciness, float newFriction)
    {
        bounciness = newBounciness;
        friction = newFriction;
        
        if (bouncyMaterial != null)
        {
            bouncyMaterial.bounciness = bounciness;
            bouncyMaterial.friction = friction;
        }
    }
}
