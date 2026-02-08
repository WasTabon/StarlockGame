using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class StarlockSetup_07_Effects : EditorWindow
{
    [MenuItem("Starlock/Setup Effects (Iteration 7)")]
    public static void SetupEffects()
    {
        if (!EditorUtility.DisplayDialog("Setup Effects",
            "This will:\n" +
            "- Add MatchEffects (particles)\n" +
            "- Add ScreenShake\n" +
            "- Add ScorePopup\n" +
            "- Add GameAudio (generated sounds)\n" +
            "- Add JuiceManager\n" +
            "- Update references\n\n" +
            "Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        SetupMatchEffects();
        SetupScreenShake();
        SetupScorePopup();
        SetupGameAudio();
        SetupJuiceManager();
        UpdateGameplayController();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Setup Complete",
            "Effects setup complete!\n\n" +
            "- MatchEffects (particles on match)\n" +
            "- ScreenShake (camera shake)\n" +
            "- ScorePopup (+100 floating text)\n" +
            "- GameAudio (procedural sounds)\n" +
            "- JuiceManager (connects everything)\n\n" +
            "Press Play to test!",
            "OK");
    }

    private static void SetupMatchEffects()
    {
        MatchEffects existing = Object.FindFirstObjectByType<MatchEffects>();
        if (existing != null)
        {
            Debug.Log("MatchEffects already exists");
            return;
        }

        GameObject obj = new GameObject("MatchEffects");
        MatchEffects effects = obj.AddComponent<MatchEffects>();

        GameObject effectsParent = new GameObject("EffectsParent");
        effectsParent.transform.SetParent(obj.transform);
        effectsParent.transform.localPosition = Vector3.zero;

        SerializedObject so = new SerializedObject(effects);
        so.FindProperty("effectsParent").objectReferenceValue = effectsParent.transform;
        so.FindProperty("particleCount").intValue = 12;
        so.FindProperty("particleSpeed").floatValue = 5f;
        so.FindProperty("particleLifetime").floatValue = 0.5f;
        so.FindProperty("particleSize").floatValue = 0.15f;
        so.ApplyModifiedProperties();

        Debug.Log("MatchEffects created");
    }

    private static void SetupScreenShake()
    {
        ScreenShake existing = Object.FindFirstObjectByType<ScreenShake>();
        if (existing != null)
        {
            Debug.Log("ScreenShake already exists");
            return;
        }

        GameObject obj = new GameObject("ScreenShake");
        ScreenShake shake = obj.AddComponent<ScreenShake>();

        SerializedObject so = new SerializedObject(shake);
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            so.FindProperty("mainCamera").objectReferenceValue = mainCam;
        }
        so.FindProperty("defaultDuration").floatValue = 0.3f;
        so.FindProperty("defaultStrength").floatValue = 0.2f;
        so.FindProperty("defaultVibrato").intValue = 20;
        so.FindProperty("defaultRandomness").floatValue = 90f;
        so.ApplyModifiedProperties();

        Debug.Log("ScreenShake created");
    }

    private static void SetupScorePopup()
    {
        ScorePopup existing = Object.FindFirstObjectByType<ScorePopup>();
        if (existing != null)
        {
            Debug.Log("ScorePopup already exists");
            return;
        }

        GameObject obj = new GameObject("ScorePopup");
        ScorePopup popup = obj.AddComponent<ScorePopup>();

        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("floatDistance").floatValue = 1f;
        so.FindProperty("duration").floatValue = 0.8f;
        so.FindProperty("fontSize").floatValue = 48f;
        so.ApplyModifiedProperties();

        Debug.Log("ScorePopup created");
    }

    private static void SetupGameAudio()
    {
        GameAudio existing = Object.FindFirstObjectByType<GameAudio>();
        if (existing != null)
        {
            Debug.Log("GameAudio already exists");
            return;
        }

        GameObject obj = new GameObject("GameAudio");
        GameAudio audio = obj.AddComponent<GameAudio>();

        SerializedObject so = new SerializedObject(audio);
        so.FindProperty("sfxVolume").floatValue = 1f;
        so.FindProperty("generateSounds").boolValue = true;
        so.ApplyModifiedProperties();

        Debug.Log("GameAudio created");
    }

    private static void SetupJuiceManager()
    {
        JuiceManager existing = Object.FindFirstObjectByType<JuiceManager>();
        if (existing != null)
        {
            Debug.Log("JuiceManager already exists, updating references...");
            ConfigureJuiceManager(existing);
            return;
        }

        GameObject obj = new GameObject("JuiceManager");
        JuiceManager juice = obj.AddComponent<JuiceManager>();
        ConfigureJuiceManager(juice);

        Debug.Log("JuiceManager created");
    }

    private static void ConfigureJuiceManager(JuiceManager juice)
    {
        SerializedObject so = new SerializedObject(juice);

        MatchEffects matchEffects = Object.FindFirstObjectByType<MatchEffects>();
        if (matchEffects != null)
        {
            so.FindProperty("matchEffects").objectReferenceValue = matchEffects;
        }

        ScreenShake screenShake = Object.FindFirstObjectByType<ScreenShake>();
        if (screenShake != null)
        {
            so.FindProperty("screenShake").objectReferenceValue = screenShake;
        }

        ScorePopup scorePopup = Object.FindFirstObjectByType<ScorePopup>();
        if (scorePopup != null)
        {
            so.FindProperty("scorePopup").objectReferenceValue = scorePopup;
        }

        GameAudio gameAudio = Object.FindFirstObjectByType<GameAudio>();
        if (gameAudio != null)
        {
            so.FindProperty("gameAudio").objectReferenceValue = gameAudio;
        }

        MatchManager matchManager = Object.FindFirstObjectByType<MatchManager>();
        if (matchManager != null)
        {
            so.FindProperty("matchManager").objectReferenceValue = matchManager;
        }

        InputManager inputManager = Object.FindFirstObjectByType<InputManager>();
        if (inputManager != null)
        {
            so.FindProperty("inputManager").objectReferenceValue = inputManager;
        }

        GameplayController gameplayController = Object.FindFirstObjectByType<GameplayController>();
        if (gameplayController != null)
        {
            so.FindProperty("gameplayController").objectReferenceValue = gameplayController;
        }

        so.FindProperty("enableEffects").boolValue = true;
        so.FindProperty("enableSounds").boolValue = true;
        so.FindProperty("enableScreenShake").boolValue = true;

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

        JuiceManager juiceManager = Object.FindFirstObjectByType<JuiceManager>();
        if (juiceManager != null)
        {
            so.FindProperty("juiceManager").objectReferenceValue = juiceManager;
        }

        so.ApplyModifiedProperties();

        Debug.Log("GameplayController updated with JuiceManager reference");
    }
}
