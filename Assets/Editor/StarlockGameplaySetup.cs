using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;

public class StarlockGameplaySetup : EditorWindow
{
    private static readonly Color CircleColor = new Color(0.4f, 0.6f, 1f, 0.8f);
    
    [MenuItem("Starlock/Setup Gameplay Objects")]
    public static void SetupGameplayObjects()
    {
        if (!EditorUtility.DisplayDialog("Setup Gameplay",
            "This will add Circle, OuterZone and Rotation system to the current Gameplay scene.\n\n" +
            "Existing objects will not be duplicated.\n\nContinue?",
            "Yes", "Cancel"))
        {
            return;
        }

        CreatePresetsFolder();
        CreateRotationPresets();
        SetupGameplayScene();

        EditorUtility.DisplayDialog("Setup Complete",
            "Gameplay objects added!\n\n" +
            "- RotationPivot with rotation\n" +
            "- CircleContainer\n" +
            "- OuterZone\n" +
            "- GameplayController configured\n\n" +
            "Press Play to test rotation.",
            "OK");
    }

    [MenuItem("Starlock/Create Rotation Presets Only")]
    public static void CreatePresetsOnly()
    {
        CreatePresetsFolder();
        CreateRotationPresets();
        EditorUtility.DisplayDialog("Done", "Rotation presets created in Assets/StarlockGame/Data/", "OK");
    }

    private static void CreatePresetsFolder()
    {
        string[] folders = new string[]
        {
            "Assets/StarlockGame",
            "Assets/StarlockGame/Data"
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

    private static void CreateRotationPresets()
    {
        CreatePreset("RotationPreset_Slow", "Slow", 30f);
        CreatePreset("RotationPreset_Medium", "Medium", 60f);
        CreatePreset("RotationPreset_Fast", "Fast", 100f);
        CreatePreset("RotationPreset_Insane", "Insane", 150f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreatePreset(string fileName, string presetName, float speed)
    {
        string path = $"Assets/StarlockGame/Data/{fileName}.asset";

        if (AssetDatabase.LoadAssetAtPath<RotationPreset>(path) != null)
        {
            Debug.Log($"Preset already exists: {path}");
            return;
        }

        RotationPreset preset = ScriptableObject.CreateInstance<RotationPreset>();

        SerializedObject so = new SerializedObject(preset);
        so.FindProperty("rotationSpeed").floatValue = speed;
        so.FindProperty("presetName").stringValue = presetName;
        so.ApplyModifiedProperties();

        AssetDatabase.CreateAsset(preset, path);
        Debug.Log($"Created preset: {path}");
    }

    private static void SetupGameplayScene()
    {
        EnsureEventSystem();
        
        GameObject rotationPivot = FindOrCreateRotationPivot();
        GameObject circleContainer = FindOrCreateCircleContainer(rotationPivot.transform);
        GameObject outerZone = FindOrCreateOuterZone(rotationPivot.transform);
        
        SetupGameplayController(rotationPivot, circleContainer, outerZone);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Debug.Log("EventSystem created");
        }
    }

    private static GameObject FindOrCreateRotationPivot()
    {
        GameObject existing = GameObject.Find("RotationPivot");
        if (existing != null)
        {
            Debug.Log("RotationPivot already exists");
            
            if (existing.GetComponent<RotationController>() == null)
            {
                existing.AddComponent<RotationController>();
            }
            
            return existing;
        }

        GameObject pivot = new GameObject("RotationPivot");
        pivot.transform.position = Vector3.zero;

        RotationController rotationController = pivot.AddComponent<RotationController>();

        RotationPreset mediumPreset = AssetDatabase.LoadAssetAtPath<RotationPreset>(
            "Assets/StarlockGame/Data/RotationPreset_Medium.asset");

        if (mediumPreset != null)
        {
            SerializedObject so = new SerializedObject(rotationController);
            so.FindProperty("currentPreset").objectReferenceValue = mediumPreset;
            so.ApplyModifiedProperties();
        }

        Debug.Log("RotationPivot created");
        return pivot;
    }

    private static GameObject FindOrCreateCircleContainer(Transform parent)
    {
        GameObject existing = GameObject.Find("CircleContainer");
        if (existing != null)
        {
            Debug.Log("CircleContainer already exists");
            return existing;
        }

        GameObject container = new GameObject("CircleContainer");
        container.transform.SetParent(parent);
        container.transform.localPosition = Vector3.zero;

        CircleContainer circleComp = container.AddComponent<CircleContainer>();

        GameObject visual = CreateCircleVisual(container.transform);

        GameObject boundary = new GameObject("CircleBoundary");
        boundary.transform.SetParent(container.transform);
        boundary.transform.localPosition = Vector3.zero;

        EdgeCollider2D edgeCollider = boundary.AddComponent<EdgeCollider2D>();
        
        Rigidbody2D rb = boundary.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        SerializedObject so = new SerializedObject(circleComp);
        so.FindProperty("circleVisual").objectReferenceValue = visual.GetComponent<SpriteRenderer>();
        so.FindProperty("boundaryCollider").objectReferenceValue = edgeCollider;
        so.FindProperty("radius").floatValue = 2f;
        so.FindProperty("maxShapesInside").intValue = 10;
        so.ApplyModifiedProperties();

        Debug.Log("CircleContainer created");
        return container;
    }

    private static GameObject CreateCircleVisual(Transform parent)
    {
        GameObject visual = new GameObject("CircleVisual");
        visual.transform.SetParent(parent);
        visual.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = CircleColor;
        sr.sortingOrder = 0;

        visual.transform.localScale = new Vector3(4f, 4f, 1f);

        return visual;
    }

    private static GameObject FindOrCreateOuterZone(Transform parent)
    {
        GameObject existing = GameObject.Find("OuterZone");
        if (existing != null)
        {
            Debug.Log("OuterZone already exists");
            return existing;
        }

        GameObject zone = new GameObject("OuterZone");
        zone.transform.SetParent(parent);
        zone.transform.localPosition = Vector3.zero;

        OuterZone outerZoneComp = zone.AddComponent<OuterZone>();

        GameObject boundary = new GameObject("OuterBoundary");
        boundary.transform.SetParent(zone.transform);
        boundary.transform.localPosition = Vector3.zero;

        EdgeCollider2D edgeCollider = boundary.AddComponent<EdgeCollider2D>();
        
        Rigidbody2D rb = boundary.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        SerializedObject so = new SerializedObject(outerZoneComp);
        so.FindProperty("outerBoundaryCollider").objectReferenceValue = edgeCollider;
        so.FindProperty("innerRadius").floatValue = 2.2f;
        so.FindProperty("outerRadius").floatValue = 4f;
        so.ApplyModifiedProperties();

        Debug.Log("OuterZone created");
        return zone;
    }

    private static void SetupGameplayController(GameObject rotationPivot, GameObject circleContainer, GameObject outerZone)
    {
        GameplayController existingController = Object.FindFirstObjectByType<GameplayController>();
        
        if (existingController == null)
        {
            GameObject controllerObj = GameObject.Find("GameplayController");
            if (controllerObj == null)
            {
                controllerObj = new GameObject("GameplayController");
            }
            existingController = controllerObj.AddComponent<GameplayController>();
            Debug.Log("GameplayController created");
        }

        RotationPreset slowPreset = AssetDatabase.LoadAssetAtPath<RotationPreset>(
            "Assets/StarlockGame/Data/RotationPreset_Slow.asset");
        RotationPreset mediumPreset = AssetDatabase.LoadAssetAtPath<RotationPreset>(
            "Assets/StarlockGame/Data/RotationPreset_Medium.asset");
        RotationPreset fastPreset = AssetDatabase.LoadAssetAtPath<RotationPreset>(
            "Assets/StarlockGame/Data/RotationPreset_Fast.asset");
        RotationPreset insanePreset = AssetDatabase.LoadAssetAtPath<RotationPreset>(
            "Assets/StarlockGame/Data/RotationPreset_Insane.asset");

        GameplayUI gameplayUI = Object.FindFirstObjectByType<GameplayUI>();

        SerializedObject so = new SerializedObject(existingController);
        so.FindProperty("rotationController").objectReferenceValue = rotationPivot.GetComponent<RotationController>();
        so.FindProperty("circleContainer").objectReferenceValue = circleContainer.GetComponent<CircleContainer>();
        so.FindProperty("outerZone").objectReferenceValue = outerZone.GetComponent<OuterZone>();
        so.FindProperty("gameplayUI").objectReferenceValue = gameplayUI;
        so.FindProperty("slowPreset").objectReferenceValue = slowPreset;
        so.FindProperty("mediumPreset").objectReferenceValue = mediumPreset;
        so.FindProperty("fastPreset").objectReferenceValue = fastPreset;
        so.FindProperty("insanePreset").objectReferenceValue = insanePreset;
        so.FindProperty("maxShapesInside").intValue = 10;
        so.ApplyModifiedProperties();

        Debug.Log("GameplayController configured");
    }

    private static Sprite CreateCircleSprite()
    {
        string spritePath = "Assets/StarlockGame/Sprites/Circle.png";

        Sprite existingSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (existingSprite != null)
        {
            return existingSprite;
        }

        if (!AssetDatabase.IsValidFolder("Assets/StarlockGame/Sprites"))
        {
            AssetDatabase.CreateFolder("Assets/StarlockGame", "Sprites");
        }

        int size = 256;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        
        Color transparent = new Color(0, 0, 0, 0);
        Color white = Color.white;
        
        float center = size / 2f;
        float outerRadius = size / 2f - 2f;
        float innerRadius = outerRadius - 8f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                
                if (dist <= outerRadius && dist >= innerRadius)
                {
                    float edgeSoftness = 2f;
                    float outerAlpha = Mathf.Clamp01((outerRadius - dist) / edgeSoftness);
                    float innerAlpha = Mathf.Clamp01((dist - innerRadius) / edgeSoftness);
                    float alpha = Mathf.Min(outerAlpha, innerAlpha);
                    
                    texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
                else
                {
                    texture.SetPixel(x, y, transparent);
                }
            }
        }

        texture.Apply();

        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(spritePath, pngData);

        AssetDatabase.Refresh();

        TextureImporter importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 64;
            importer.filterMode = FilterMode.Bilinear;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
    }
}
