using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class StarlockSetup_04_Physics : EditorWindow
{
    [MenuItem("Starlock/Setup Physics (Iteration 4)")]
    public static void SetupPhysics()
    {
        if (!EditorUtility.DisplayDialog("Setup Physics",
            "This will:\n" +
            "- Add OuterZonePhysics component\n" +
            "- Add ShapeSpawner component\n" +
            "- Apply bouncy material to boundaries\n" +
            "- Configure references\n\n" +
            "Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        SetupOuterZonePhysics();
        SetupShapeSpawner();
        ApplyBouncyMaterialToBoundaries();
        UpdateShapeFactorySettings();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Setup Complete",
            "Physics setup complete!\n\n" +
            "- OuterZonePhysics added\n" +
            "- ShapeSpawner added (5 pairs on start)\n" +
            "- Bouncy material applied to boundaries\n\n" +
            "Press Play to test!",
            "OK");
    }

    private static void SetupOuterZonePhysics()
    {
        OuterZonePhysics existing = Object.FindFirstObjectByType<OuterZonePhysics>();
        if (existing != null)
        {
            Debug.Log("OuterZonePhysics already exists, updating references...");
            ConfigureOuterZonePhysics(existing);
            return;
        }

        GameObject physicsObj = new GameObject("OuterZonePhysics");
        OuterZonePhysics physics = physicsObj.AddComponent<OuterZonePhysics>();
        ConfigureOuterZonePhysics(physics);

        Debug.Log("OuterZonePhysics created");
    }

    private static void ConfigureOuterZonePhysics(OuterZonePhysics physics)
    {
        SerializedObject so = new SerializedObject(physics);

        RotationController rotationController = Object.FindFirstObjectByType<RotationController>();
        if (rotationController != null)
        {
            so.FindProperty("rotationController").objectReferenceValue = rotationController;
        }

        OuterZone outerZone = Object.FindFirstObjectByType<OuterZone>();
        if (outerZone != null)
        {
            so.FindProperty("outerZone").objectReferenceValue = outerZone;
        }

        CircleContainer circleContainer = Object.FindFirstObjectByType<CircleContainer>();
        if (circleContainer != null)
        {
            so.FindProperty("circleContainer").objectReferenceValue = circleContainer;
        }

        so.FindProperty("tangentialForceMultiplier").floatValue = 2f;
        so.FindProperty("centrifugalForceMultiplier").floatValue = 0.5f;
        so.FindProperty("maxForce").floatValue = 10f;
        so.FindProperty("velocityDamping").floatValue = 0.98f;
        so.FindProperty("maxVelocity").floatValue = 8f;

        so.ApplyModifiedProperties();
    }

    private static void SetupShapeSpawner()
    {
        ShapeSpawner existing = Object.FindFirstObjectByType<ShapeSpawner>();
        if (existing != null)
        {
            Debug.Log("ShapeSpawner already exists, updating references...");
            ConfigureShapeSpawner(existing);
            return;
        }

        GameObject spawnerObj = new GameObject("ShapeSpawner");
        ShapeSpawner spawner = spawnerObj.AddComponent<ShapeSpawner>();
        ConfigureShapeSpawner(spawner);

        Debug.Log("ShapeSpawner created");
    }

    private static void ConfigureShapeSpawner(ShapeSpawner spawner)
    {
        SerializedObject so = new SerializedObject(spawner);

        ShapeFactory factory = Object.FindFirstObjectByType<ShapeFactory>();
        if (factory != null)
        {
            so.FindProperty("shapeFactory").objectReferenceValue = factory;
        }
        else
        {
            Debug.LogWarning("ShapeFactory not found! Run 'Starlock > Setup Shapes' first.");
        }

        OuterZone outerZone = Object.FindFirstObjectByType<OuterZone>();
        if (outerZone != null)
        {
            so.FindProperty("outerZone").objectReferenceValue = outerZone;
        }

        OuterZonePhysics physics = Object.FindFirstObjectByType<OuterZonePhysics>();
        if (physics != null)
        {
            so.FindProperty("outerZonePhysics").objectReferenceValue = physics;
        }

        so.FindProperty("pairsToSpawn").intValue = 5;
        so.FindProperty("minDistanceBetweenShapes").floatValue = 0.5f;
        so.FindProperty("spawnOnStart").boolValue = true;

        so.ApplyModifiedProperties();
    }

    private static void ApplyBouncyMaterialToBoundaries()
    {
        PhysicsMaterial2D bouncyMaterial = new PhysicsMaterial2D("BoundaryBouncyMaterial");
        bouncyMaterial.bounciness = 0.8f;
        bouncyMaterial.friction = 0.1f;

        string materialPath = "Assets/StarlockGame/Physics/BoundaryBouncyMaterial.asset";
        
        if (!AssetDatabase.IsValidFolder("Assets/StarlockGame/Physics"))
        {
            AssetDatabase.CreateFolder("Assets/StarlockGame", "Physics");
        }

        PhysicsMaterial2D existingMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(materialPath);
        if (existingMaterial == null)
        {
            AssetDatabase.CreateAsset(bouncyMaterial, materialPath);
            AssetDatabase.SaveAssets();
        }
        else
        {
            bouncyMaterial = existingMaterial;
        }

        GameObject circleBoundary = GameObject.Find("CircleBoundary");
        if (circleBoundary != null)
        {
            EdgeCollider2D col = circleBoundary.GetComponent<EdgeCollider2D>();
            if (col != null)
            {
                col.sharedMaterial = bouncyMaterial;
                Debug.Log("Applied bouncy material to CircleBoundary");
            }
        }

        GameObject outerBoundary = GameObject.Find("OuterBoundary");
        if (outerBoundary != null)
        {
            EdgeCollider2D col = outerBoundary.GetComponent<EdgeCollider2D>();
            if (col != null)
            {
                col.sharedMaterial = bouncyMaterial;
                Debug.Log("Applied bouncy material to OuterBoundary");
            }
        }
    }

    private static void UpdateShapeFactorySettings()
    {
        ShapeFactory factory = Object.FindFirstObjectByType<ShapeFactory>();
        if (factory == null)
        {
            Debug.LogWarning("ShapeFactory not found!");
            return;
        }

        SerializedObject so = new SerializedObject(factory);
        so.FindProperty("bounciness").floatValue = 0.8f;
        so.FindProperty("friction").floatValue = 0.1f;
        so.ApplyModifiedProperties();

        Debug.Log("ShapeFactory physics settings updated");
    }

    [MenuItem("Starlock/Test: Respawn All Shapes")]
    public static void TestRespawnShapes()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Error", "Enter Play mode first to test.", "OK");
            return;
        }

        ShapeSpawner spawner = Object.FindFirstObjectByType<ShapeSpawner>();
        if (spawner == null)
        {
            EditorUtility.DisplayDialog("Error", "ShapeSpawner not found!\n\nRun 'Starlock > Setup Physics' first.", "OK");
            return;
        }

        spawner.SpawnInitialShapes();
        Debug.Log("Shapes respawned!");
    }
}
