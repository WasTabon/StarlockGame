using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public class StarlockSetup_03_Shapes : EditorWindow
{
    [MenuItem("Starlock/Setup Shapes")]
    public static void SetupShapes()
    {
        if (!EditorUtility.DisplayDialog("Setup Shapes",
            "This will:\n" +
            "- Generate sprite textures for all 5 shapes\n" +
            "- Create Shape prefab\n" +
            "- Add ShapeFactory to scene\n\n" +
            "Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        CreateFolders();
        GenerateShapeSprites();
        CreateShapePrefab();
        AddShapeFactoryToScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Setup Complete",
            "Shapes setup complete!\n\n" +
            "- 5 shape sprites in Assets/StarlockGame/Sprites/Shapes/\n" +
            "- Shape prefab in Assets/StarlockGame/Prefabs/\n" +
            "- ShapeFactory added to scene\n\n" +
            "You can now create shapes via ShapeFactory.",
            "OK");
    }

    [MenuItem("Starlock/Generate Shape Sprites Only")]
    public static void GenerateSpritesOnly()
    {
        CreateFolders();
        GenerateShapeSprites();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Done", "Shape sprites generated in Assets/StarlockGame/Sprites/Shapes/", "OK");
    }

    private static void CreateFolders()
    {
        string[] folders = new string[]
        {
            "Assets/StarlockGame",
            "Assets/StarlockGame/Sprites",
            "Assets/StarlockGame/Sprites/Shapes",
            "Assets/StarlockGame/Prefabs",
            "Assets/Resources",
            "Assets/Resources/Shapes"
        };

        foreach (var folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parent = Path.GetDirectoryName(folder).Replace("\\", "/");
                string newFolder = Path.GetFileName(folder);
                AssetDatabase.CreateFolder(parent, newFolder);
            }
        }

        AssetDatabase.Refresh();
    }

    private static void GenerateShapeSprites()
    {
        foreach (ShapeType shapeType in System.Enum.GetValues(typeof(ShapeType)))
        {
            GenerateSpriteForType(shapeType);
        }

        Debug.Log("All shape sprites generated!");
    }

    private static void GenerateSpriteForType(ShapeType shapeType)
    {
        string spritePath = $"Assets/StarlockGame/Sprites/Shapes/{shapeType}.png";
        string resourcePath = $"Assets/Resources/Shapes/{shapeType}.png";

        Texture2D texture = ShapeVisualGenerator.GenerateShapeTexture(shapeType);
        byte[] pngData = texture.EncodeToPNG();

        File.WriteAllBytes(spritePath, pngData);
        File.WriteAllBytes(resourcePath, pngData);

        AssetDatabase.Refresh();

        ConfigureTextureImporter(spritePath);
        ConfigureTextureImporter(resourcePath);

        Debug.Log($"Generated sprite: {shapeType}");
    }

    private static void ConfigureTextureImporter(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 64;
            importer.filterMode = FilterMode.Bilinear;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }

    private static void CreateShapePrefab()
    {
        string prefabPath = "Assets/StarlockGame/Prefabs/Shape.prefab";

        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            Debug.Log("Shape prefab already exists, skipping...");
            return;
        }

        GameObject shapeObj = new GameObject("Shape");

        SpriteRenderer sr = shapeObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 10;

        Rigidbody2D rb = shapeObj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        PolygonCollider2D col = shapeObj.AddComponent<PolygonCollider2D>();

        shapeObj.AddComponent<Shape>();

        PrefabUtility.SaveAsPrefabAsset(shapeObj, prefabPath);
        Object.DestroyImmediate(shapeObj);

        Debug.Log($"Created Shape prefab at: {prefabPath}");
    }

    private static void AddShapeFactoryToScene()
    {
        ShapeFactory existingFactory = Object.FindFirstObjectByType<ShapeFactory>();
        if (existingFactory != null)
        {
            Debug.Log("ShapeFactory already exists in scene, updating...");
            SetupFactoryReferences(existingFactory);
            return;
        }

        GameObject factoryObj = new GameObject("ShapeFactory");
        ShapeFactory factory = factoryObj.AddComponent<ShapeFactory>();
        
        SetupFactoryReferences(factory);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("ShapeFactory added to scene");
    }

    private static void SetupFactoryReferences(ShapeFactory factory)
    {
        SerializedObject so = new SerializedObject(factory);

        GameObject rotationPivot = GameObject.Find("RotationPivot");
        if (rotationPivot != null)
        {
            Transform shapesParent = rotationPivot.transform.Find("ShapesParent");
            if (shapesParent == null)
            {
                GameObject shapesParentObj = new GameObject("ShapesParent");
                shapesParentObj.transform.SetParent(rotationPivot.transform);
                shapesParentObj.transform.localPosition = Vector3.zero;
                shapesParent = shapesParentObj.transform;
            }

            so.FindProperty("shapesParent").objectReferenceValue = shapesParent;
        }
        else
        {
            Debug.LogWarning("RotationPivot not found! Run 'Starlock > Setup Gameplay Objects' first.");
        }

        so.FindProperty("shapeSize").floatValue = 0.4f;
        so.ApplyModifiedProperties();
    }

    [MenuItem("Starlock/Test: Spawn Random Shape")]
    public static void TestSpawnRandomShape()
    {
        ShapeFactory factory = Object.FindFirstObjectByType<ShapeFactory>();
        if (factory == null)
        {
            EditorUtility.DisplayDialog("Error", "ShapeFactory not found in scene!\n\nRun 'Starlock > Setup Shapes' first.", "OK");
            return;
        }

        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Error", "Enter Play mode first to test shape spawning.", "OK");
            return;
        }

        factory.CreateRandomShape(Vector3.zero);
        Debug.Log("Random shape spawned at (0,0,0)");
    }
}
