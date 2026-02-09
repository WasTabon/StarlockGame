using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StarlockSetup_08_Progress : EditorWindow
{
    [MenuItem("Starlock/Setup Progress System (Iteration 8)")]
    public static void SetupProgressSystem()
    {
        if (!EditorUtility.DisplayDialog("Setup Progress System",
            "This will:\n" +
            "- Add ProgressManager singleton\n" +
            "- Update level buttons with LevelButton component\n" +
            "- Add reset progress button to settings\n\n" +
            "Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        SetupProgressManager();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Setup Complete",
            "Progress system setup complete!\n\n" +
            "- ProgressManager added\n\n" +
            "For MainMenu scene:\n" +
            "- Add LevelButton component to each level button\n" +
            "- Or run 'Setup Level Buttons' in MainMenu scene\n\n" +
            "Press Play to test!",
            "OK");
    }

    private static void SetupProgressManager()
    {
        ProgressManager existing = Object.FindFirstObjectByType<ProgressManager>();
        if (existing != null)
        {
            Debug.Log("ProgressManager already exists");
            return;
        }

        GameObject obj = new GameObject("ProgressManager");
        obj.AddComponent<ProgressManager>();

        Debug.Log("ProgressManager created");
    }

    [MenuItem("Starlock/Setup Level Buttons (MainMenu)")]
    public static void SetupLevelButtons()
    {
        MainMenuUI mainMenuUI = Object.FindFirstObjectByType<MainMenuUI>();
        if (mainMenuUI == null)
        {
            EditorUtility.DisplayDialog("Error", "MainMenuUI not found!\n\nMake sure you have MainMenu scene open.", "OK");
            return;
        }

        SerializedObject so = new SerializedObject(mainMenuUI);
        SerializedProperty containerProp = so.FindProperty("levelButtonsContainer");
        
        if (containerProp.objectReferenceValue == null)
        {
            EditorUtility.DisplayDialog("Error", "levelButtonsContainer not assigned in MainMenuUI!", "OK");
            return;
        }

        Transform container = containerProp.objectReferenceValue as Transform;
        if (container == null)
        {
            EditorUtility.DisplayDialog("Error", "levelButtonsContainer is not a Transform!", "OK");
            return;
        }

        int buttonsSetup = 0;
        Button[] buttons = container.GetComponentsInChildren<Button>(true);
        
        for (int i = 0; i < buttons.Length; i++)
        {
            Button btn = buttons[i];
            LevelButton levelButton = btn.GetComponent<LevelButton>();
            
            if (levelButton == null)
            {
                levelButton = btn.gameObject.AddComponent<LevelButton>();
            }

            SerializedObject lbSo = new SerializedObject(levelButton);
            lbSo.FindProperty("button").objectReferenceValue = btn;

            TextMeshProUGUI numberText = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (numberText != null)
            {
                lbSo.FindProperty("levelNumberText").objectReferenceValue = numberText;
            }

            lbSo.ApplyModifiedProperties();
            buttonsSetup++;
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Setup Complete",
            $"Level buttons setup complete!\n\n" +
            $"- {buttonsSetup} buttons configured with LevelButton\n\n" +
            "Note: For full visual setup (locked overlay, stars),\n" +
            "manually add child objects to each button.",
            "OK");
    }

    [MenuItem("Starlock/Debug: Reset All Progress")]
    public static void ResetAllProgress()
    {
        if (!EditorUtility.DisplayDialog("Reset Progress",
            "This will reset ALL player progress!\n\n" +
            "Are you sure?",
            "Yes, Reset", "Cancel"))
        {
            return;
        }

        for (int i = 1; i <= 20; i++)
        {
            PlayerPrefs.DeleteKey("LevelCompleted_" + i);
            PlayerPrefs.DeleteKey("LevelStars_" + i);
            PlayerPrefs.DeleteKey("LevelHighscore_" + i);
        }
        PlayerPrefs.SetInt("MaxUnlockedLevel", 1);
        PlayerPrefs.Save();

        Debug.Log("All progress reset via Editor!");
        
        EditorUtility.DisplayDialog("Done", "All progress has been reset.", "OK");
    }

    [MenuItem("Starlock/Debug: Unlock All Levels")]
    public static void UnlockAllLevels()
    {
        PlayerPrefs.SetInt("MaxUnlockedLevel", 10);
        PlayerPrefs.Save();

        Debug.Log("All levels unlocked via Editor!");
        
        EditorUtility.DisplayDialog("Done", "All levels have been unlocked.", "OK");
    }
}
