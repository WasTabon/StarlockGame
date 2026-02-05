using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class StarlockSetup_05_Input : EditorWindow
{
    [MenuItem("Starlock/Setup Input & Matching (Iteration 5)")]
    public static void SetupInputAndMatching()
    {
        if (!EditorUtility.DisplayDialog("Setup Input & Matching",
            "This will:\n" +
            "- Add InputManager\n" +
            "- Add MatchManager\n" +
            "- Update GameplayController references\n\n" +
            "Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        SetupInputManager();
        SetupMatchManager();
        UpdateGameplayController();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Setup Complete",
            "Input & Matching setup complete!\n\n" +
            "- InputManager added\n" +
            "- MatchManager added\n" +
            "- GameplayController updated\n\n" +
            "Press Play and tap shapes to test!",
            "OK");
    }

    private static void SetupInputManager()
    {
        InputManager existing = Object.FindFirstObjectByType<InputManager>();
        if (existing != null)
        {
            Debug.Log("InputManager already exists, updating references...");
            ConfigureInputManager(existing);
            return;
        }

        GameObject inputObj = new GameObject("InputManager");
        InputManager input = inputObj.AddComponent<InputManager>();
        ConfigureInputManager(input);

        Debug.Log("InputManager created");
    }

    private static void ConfigureInputManager(InputManager input)
    {
        SerializedObject so = new SerializedObject(input);

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            so.FindProperty("mainCamera").objectReferenceValue = mainCamera;
        }

        CircleContainer circleContainer = Object.FindFirstObjectByType<CircleContainer>();
        if (circleContainer != null)
        {
            so.FindProperty("circleContainer").objectReferenceValue = circleContainer;
        }

        OuterZonePhysics outerZonePhysics = Object.FindFirstObjectByType<OuterZonePhysics>();
        if (outerZonePhysics != null)
        {
            so.FindProperty("outerZonePhysics").objectReferenceValue = outerZonePhysics;
        }

        so.FindProperty("tapRadius").floatValue = 0.5f;
        so.FindProperty("showDebugInfo").boolValue = true;

        so.ApplyModifiedProperties();
    }

    private static void SetupMatchManager()
    {
        MatchManager existing = Object.FindFirstObjectByType<MatchManager>();
        if (existing != null)
        {
            Debug.Log("MatchManager already exists, updating references...");
            ConfigureMatchManager(existing);
            return;
        }

        GameObject matchObj = new GameObject("MatchManager");
        MatchManager match = matchObj.AddComponent<MatchManager>();
        ConfigureMatchManager(match);

        Debug.Log("MatchManager created");
    }

    private static void ConfigureMatchManager(MatchManager match)
    {
        SerializedObject so = new SerializedObject(match);

        CircleContainer circleContainer = Object.FindFirstObjectByType<CircleContainer>();
        if (circleContainer != null)
        {
            so.FindProperty("circleContainer").objectReferenceValue = circleContainer;
        }

        so.FindProperty("matchCheckDelay").floatValue = 0.1f;
        so.FindProperty("pointsPerMatch").intValue = 100;
        so.FindProperty("showDebugInfo").boolValue = true;

        so.ApplyModifiedProperties();
    }

    private static void UpdateGameplayController()
    {
        GameplayController controller = Object.FindFirstObjectByType<GameplayController>();
        if (controller == null)
        {
            Debug.LogWarning("GameplayController not found!");
            return;
        }

        SerializedObject so = new SerializedObject(controller);

        InputManager inputManager = Object.FindFirstObjectByType<InputManager>();
        if (inputManager != null)
        {
            so.FindProperty("inputManager").objectReferenceValue = inputManager;
        }

        MatchManager matchManager = Object.FindFirstObjectByType<MatchManager>();
        if (matchManager != null)
        {
            so.FindProperty("matchManager").objectReferenceValue = matchManager;
        }

        ShapeSpawner shapeSpawner = Object.FindFirstObjectByType<ShapeSpawner>();
        if (shapeSpawner != null)
        {
            so.FindProperty("shapeSpawner").objectReferenceValue = shapeSpawner;
        }

        GameplayUI gameplayUI = Object.FindFirstObjectByType<GameplayUI>();
        if (gameplayUI != null)
        {
            so.FindProperty("gameplayUI").objectReferenceValue = gameplayUI;
        }

        RotationController rotationController = Object.FindFirstObjectByType<RotationController>();
        if (rotationController != null)
        {
            so.FindProperty("rotationController").objectReferenceValue = rotationController;
        }

        CircleContainer circleContainer = Object.FindFirstObjectByType<CircleContainer>();
        if (circleContainer != null)
        {
            so.FindProperty("circleContainer").objectReferenceValue = circleContainer;
        }

        OuterZone outerZone = Object.FindFirstObjectByType<OuterZone>();
        if (outerZone != null)
        {
            so.FindProperty("outerZone").objectReferenceValue = outerZone;
        }

        so.ApplyModifiedProperties();

        Debug.Log("GameplayController references updated");
    }
}
