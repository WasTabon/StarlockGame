using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class StarlockSetup_09_Endless : EditorWindow
{
    [MenuItem("Starlock/Setup Endless Mode (Iteration 9)")]
    public static void SetupEndlessMode()
    {
        if (!EditorUtility.DisplayDialog("Setup Endless Mode",
            "This will:\n" +
            "- Add EndlessManager\n" +
            "- Add HighscoreManager\n" +
            "- Update references\n\n" +
            "Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        SetupEndlessManager();
        SetupHighscoreManager();
        UpdateGameplayController();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Setup Complete",
            "Endless mode setup complete!\n\n" +
            "- EndlessManager added\n" +
            "- HighscoreManager added\n\n" +
            "Test:\n" +
            "1. Go to MainMenu scene\n" +
            "2. Click 'Endless' button\n" +
            "3. Shapes will spawn continuously\n" +
            "4. Difficulty increases over time",
            "OK");
    }

    private static void SetupEndlessManager()
    {
        EndlessManager existing = Object.FindFirstObjectByType<EndlessManager>();
        if (existing != null)
        {
            Debug.Log("EndlessManager already exists, updating references...");
            ConfigureEndlessManager(existing);
            return;
        }

        GameObject obj = new GameObject("EndlessManager");
        EndlessManager manager = obj.AddComponent<EndlessManager>();
        ConfigureEndlessManager(manager);

        Debug.Log("EndlessManager created");
    }

    private static void ConfigureEndlessManager(EndlessManager manager)
    {
        SerializedObject so = new SerializedObject(manager);

        ShapeSpawner shapeSpawner = Object.FindFirstObjectByType<ShapeSpawner>();
        if (shapeSpawner != null)
        {
            so.FindProperty("shapeSpawner").objectReferenceValue = shapeSpawner;
        }

        RotationController rotationController = Object.FindFirstObjectByType<RotationController>();
        if (rotationController != null)
        {
            so.FindProperty("rotationController").objectReferenceValue = rotationController;
        }

        MatchManager matchManager = Object.FindFirstObjectByType<MatchManager>();
        if (matchManager != null)
        {
            so.FindProperty("matchManager").objectReferenceValue = matchManager;
        }

        so.FindProperty("initialSpawnInterval").floatValue = 3f;
        so.FindProperty("minSpawnInterval").floatValue = 1f;
        so.FindProperty("spawnIntervalDecreaseRate").floatValue = 0.05f;
        so.FindProperty("maxShapesOnScreen").intValue = 20;
        so.FindProperty("initialRotationSpeed").floatValue = 30f;
        so.FindProperty("maxRotationSpeed").floatValue = 100f;
        so.FindProperty("rotationSpeedIncreaseRate").floatValue = 1f;
        so.FindProperty("difficultyIncreaseInterval").floatValue = 10f;

        so.ApplyModifiedProperties();
    }

    private static void SetupHighscoreManager()
    {
        HighscoreManager existing = Object.FindFirstObjectByType<HighscoreManager>();
        if (existing != null)
        {
            Debug.Log("HighscoreManager already exists");
            return;
        }

        GameObject obj = new GameObject("HighscoreManager");
        obj.AddComponent<HighscoreManager>();

        Debug.Log("HighscoreManager created");
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

        EndlessManager endlessManager = Object.FindFirstObjectByType<EndlessManager>();
        if (endlessManager != null)
        {
            so.FindProperty("endlessManager").objectReferenceValue = endlessManager;
        }

        so.ApplyModifiedProperties();

        Debug.Log("GameplayController updated with EndlessManager reference");
    }

    [MenuItem("Starlock/Debug: Reset Endless Highscores")]
    public static void ResetEndlessHighscores()
    {
        if (!EditorUtility.DisplayDialog("Reset Highscores",
            "This will reset ALL endless highscores!\n\n" +
            "Are you sure?",
            "Yes, Reset", "Cancel"))
        {
            return;
        }

        PlayerPrefs.DeleteKey("EndlessHighscore");
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.DeleteKey("EndlessHighscores_" + i + "_score");
            PlayerPrefs.DeleteKey("EndlessHighscores_" + i + "_time");
        }
        PlayerPrefs.Save();

        Debug.Log("Endless highscores reset via Editor!");
        
        EditorUtility.DisplayDialog("Done", "Endless highscores have been reset.", "OK");
    }
}
