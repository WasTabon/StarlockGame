using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StarlockSetup_10_Final : EditorWindow
{
    [MenuItem("Starlock/Setup Gameplay Scene (Iteration 10)")]
    public static void SetupGameplayScene()
    {
        if (!EditorUtility.DisplayDialog("Setup Gameplay Scene",
            "This will:\n" +
            "- Create Pause popup\n" +
            "- Update references\n\n" +
            "Make sure you have Gameplay scene open!\n\n" +
            "Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        CreatePausePopup();
        UpdateGameplayUI();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Setup Complete",
            "Gameplay scene setup complete!\n\n" +
            "- PausePopup created\n\n" +
            "Don't forget to:\n" +
            "1. Assign pauseButton in GameplayUI\n" +
            "2. Save the scene",
            "OK");
    }

    [MenuItem("Starlock/Setup MainMenu Tutorial (Iteration 10)")]
    public static void SetupMainMenuTutorial()
    {
        if (!EditorUtility.DisplayDialog("Setup MainMenu Tutorial",
            "This will:\n" +
            "- Add TutorialManager\n" +
            "- Create Tutorial panel\n" +
            "- Update MainMenuUI references\n\n" +
            "Make sure you have MainMenu scene open!\n\n" +
            "Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        SetupTutorialManager();
        CreateTutorialPanel();
        UpdateMainMenuUI();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Setup Complete",
            "MainMenu tutorial setup complete!\n\n" +
            "- TutorialManager added\n" +
            "- TutorialPanel created\n\n" +
            "Test:\n" +
            "1. Reset tutorial via Debug menu\n" +
            "2. Start game - tutorial should appear",
            "OK");
    }

    private static void SetupTutorialManager()
    {
        TutorialManager existing = Object.FindFirstObjectByType<TutorialManager>();
        if (existing != null)
        {
            Debug.Log("TutorialManager already exists");
            return;
        }

        GameObject obj = new GameObject("TutorialManager");
        obj.AddComponent<TutorialManager>();

        Debug.Log("TutorialManager created");
    }

    private static void CreatePausePopup()
    {
        PausePopup existingPopup = Object.FindFirstObjectByType<PausePopup>(FindObjectsInactive.Include);
        if (existingPopup != null)
        {
            Debug.Log("PausePopup already exists");
            return;
        }

        Canvas canvas = FindUICanvas();
        if (canvas == null)
        {
            Debug.LogWarning("UI_Canvas not found!");
            return;
        }

        GameObject popup = CreatePopupBase(canvas.transform, "PausePopup");
        PausePopup pauseComp = popup.AddComponent<PausePopup>();

        Transform panelBg = popup.transform.Find("PanelBackground");

        GameObject title = CreateText("Title", panelBg, "PAUSED", 64, FontStyles.Bold, Color.white);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.75f);
        titleRect.anchorMax = new Vector2(1, 0.9f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        GameObject resumeBtn = CreateButton("ResumeButton", panelBg, "RESUME", new Color(0.3f, 0.8f, 0.5f));
        RectTransform resumeRect = resumeBtn.GetComponent<RectTransform>();
        resumeRect.anchorMin = new Vector2(0.1f, 0.5f);
        resumeRect.anchorMax = new Vector2(0.9f, 0.65f);
        resumeRect.offsetMin = Vector2.zero;
        resumeRect.offsetMax = Vector2.zero;

        GameObject restartBtn = CreateButton("RestartButton", panelBg, "RESTART", new Color(1f, 0.6f, 0.3f));
        RectTransform restartRect = restartBtn.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.1f, 0.3f);
        restartRect.anchorMax = new Vector2(0.9f, 0.45f);
        restartRect.offsetMin = Vector2.zero;
        restartRect.offsetMax = Vector2.zero;

        GameObject menuBtn = CreateButton("MenuButton", panelBg, "MENU", new Color(0.4f, 0.4f, 0.5f));
        RectTransform menuRect = menuBtn.GetComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0.1f, 0.1f);
        menuRect.anchorMax = new Vector2(0.9f, 0.25f);
        menuRect.offsetMin = Vector2.zero;
        menuRect.offsetMax = Vector2.zero;

        SerializedObject so = new SerializedObject(pauseComp);
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("resumeButton").objectReferenceValue = resumeBtn.GetComponent<Button>();
        so.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
        so.FindProperty("menuButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        popup.SetActive(false);

        Debug.Log("PausePopup created");
    }

    private static void CreateTutorialPanel()
    {
        GameObject existing = GameObject.Find("TutorialPanel");
        if (existing != null)
        {
            Debug.Log("TutorialPanel already exists");
            return;
        }

        Canvas canvas = FindUICanvas();
        if (canvas == null)
        {
            Debug.LogWarning("UI_Canvas not found!");
            return;
        }

        GameObject panel = new GameObject("TutorialPanel");
        panel.transform.SetParent(canvas.transform);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelRect.localPosition = Vector3.zero;
        panelRect.localScale = Vector3.one;

        CanvasGroup cg = panel.AddComponent<CanvasGroup>();

        GameObject dimmer = new GameObject("Dimmer");
        dimmer.transform.SetParent(panel.transform);
        RectTransform dimmerRect = dimmer.AddComponent<RectTransform>();
        dimmerRect.anchorMin = Vector2.zero;
        dimmerRect.anchorMax = Vector2.one;
        dimmerRect.offsetMin = Vector2.zero;
        dimmerRect.offsetMax = Vector2.zero;
        dimmerRect.localPosition = Vector3.zero;
        dimmerRect.localScale = Vector3.one;

        Image dimmerImage = dimmer.AddComponent<Image>();
        dimmerImage.color = new Color(0, 0, 0, 0.85f);
        dimmerImage.raycastTarget = true;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(panel.transform);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.1f, 0.3f);
        contentRect.anchorMax = new Vector2(0.9f, 0.7f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        contentRect.localPosition = Vector3.zero;
        contentRect.localScale = Vector3.one;

        Image contentBg = content.AddComponent<Image>();
        contentBg.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);

        GameObject tutorialText = CreateText("TutorialText", content.transform, "Welcome to Starlock!", 36, FontStyles.Normal, Color.white);
        RectTransform textRect = tutorialText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.05f, 0.35f);
        textRect.anchorMax = new Vector2(0.95f, 0.95f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        tutorialText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.TopLeft;

        GameObject skipBtn = CreateButton("SkipButton", content.transform, "SKIP", new Color(0.4f, 0.4f, 0.5f));
        RectTransform skipRect = skipBtn.GetComponent<RectTransform>();
        skipRect.anchorMin = new Vector2(0.05f, 0.05f);
        skipRect.anchorMax = new Vector2(0.45f, 0.25f);
        skipRect.offsetMin = Vector2.zero;
        skipRect.offsetMax = Vector2.zero;

        GameObject nextBtn = CreateButton("NextButton", content.transform, "NEXT", new Color(0.3f, 0.8f, 0.5f));
        RectTransform nextRect = nextBtn.GetComponent<RectTransform>();
        nextRect.anchorMin = new Vector2(0.55f, 0.05f);
        nextRect.anchorMax = new Vector2(0.95f, 0.25f);
        nextRect.offsetMin = Vector2.zero;
        nextRect.offsetMax = Vector2.zero;

        TutorialManager tutorialManager = Object.FindFirstObjectByType<TutorialManager>();
        if (tutorialManager != null)
        {
            SerializedObject so = new SerializedObject(tutorialManager);
            so.FindProperty("tutorialPanel").objectReferenceValue = panel;
            so.FindProperty("tutorialText").objectReferenceValue = tutorialText.GetComponent<TextMeshProUGUI>();
            so.FindProperty("skipButton").objectReferenceValue = skipBtn.GetComponent<Button>();
            so.FindProperty("nextButton").objectReferenceValue = nextBtn.GetComponent<Button>();
            so.ApplyModifiedProperties();
        }

        panel.SetActive(false);

        Debug.Log("TutorialPanel created");
    }

    private static Canvas FindUICanvas()
    {
        GameObject canvasObj = GameObject.Find("UI_Canvas");
        if (canvasObj != null)
        {
            return canvasObj.GetComponent<Canvas>();
        }

        return Object.FindFirstObjectByType<Canvas>();
    }

    private static GameObject CreatePopupBase(Transform parent, string name)
    {
        GameObject popup = new GameObject(name);
        popup.transform.SetParent(parent);

        RectTransform popupRect = popup.AddComponent<RectTransform>();
        popupRect.anchorMin = Vector2.zero;
        popupRect.anchorMax = Vector2.one;
        popupRect.offsetMin = Vector2.zero;
        popupRect.offsetMax = Vector2.zero;
        popupRect.localPosition = Vector3.zero;
        popupRect.localScale = Vector3.one;

        popup.AddComponent<CanvasGroup>();

        GameObject dimmer = new GameObject("Dimmer");
        dimmer.transform.SetParent(popup.transform);
        RectTransform dimmerRect = dimmer.AddComponent<RectTransform>();
        dimmerRect.anchorMin = Vector2.zero;
        dimmerRect.anchorMax = Vector2.one;
        dimmerRect.offsetMin = Vector2.zero;
        dimmerRect.offsetMax = Vector2.zero;
        dimmerRect.localPosition = Vector3.zero;
        dimmerRect.localScale = Vector3.one;

        Image dimmerImage = dimmer.AddComponent<Image>();
        dimmerImage.color = new Color(0, 0, 0, 0.7f);
        dimmerImage.raycastTarget = true;

        GameObject panelBg = new GameObject("PanelBackground");
        panelBg.transform.SetParent(popup.transform);
        RectTransform bgRect = panelBg.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.1f, 0.25f);
        bgRect.anchorMax = new Vector2(0.9f, 0.75f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bgRect.localPosition = Vector3.zero;
        bgRect.localScale = Vector3.one;

        Image bgImage = panelBg.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);
        bgImage.raycastTarget = true;

        return popup;
    }

    private static GameObject CreateText(string name, Transform parent, string text, float fontSize, FontStyles style, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;

        return textObj;
    }

    private static GameObject CreateButton(string name, Transform parent, string text, Color bgColor)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;

        Image image = buttonObj.AddComponent<Image>();
        image.color = bgColor;

        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f);
        colors.pressedColor = new Color(0.9f, 0.9f, 0.9f);
        button.colors = colors;

        GameObject textChild = CreateText("Text", buttonObj.transform, text, 32, FontStyles.Bold, Color.white);
        RectTransform textRect = textChild.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return buttonObj;
    }

    private static void UpdateGameplayUI()
    {
        GameplayUI gameplayUI = Object.FindFirstObjectByType<GameplayUI>();
        if (gameplayUI == null)
        {
            Debug.LogWarning("GameplayUI not found!");
            return;
        }

        SerializedObject so = new SerializedObject(gameplayUI);

        PausePopup pausePopup = Object.FindFirstObjectByType<PausePopup>(FindObjectsInactive.Include);
        if (pausePopup != null)
        {
            so.FindProperty("pausePopup").objectReferenceValue = pausePopup;
        }

        so.ApplyModifiedProperties();

        Debug.Log("GameplayUI updated with PausePopup reference");
    }

    private static void UpdateMainMenuUI()
    {
        MainMenuUI mainMenuUI = Object.FindFirstObjectByType<MainMenuUI>();
        if (mainMenuUI == null)
        {
            Debug.LogWarning("MainMenuUI not found!");
            return;
        }

        SerializedObject so = new SerializedObject(mainMenuUI);

        TutorialManager tutorialManager = Object.FindFirstObjectByType<TutorialManager>();
        if (tutorialManager != null)
        {
            so.FindProperty("tutorialManager").objectReferenceValue = tutorialManager;
        }

        so.ApplyModifiedProperties();

        Debug.Log("MainMenuUI updated with TutorialManager reference");
    }

    [MenuItem("Starlock/Debug: Reset Tutorial")]
    public static void ResetTutorial()
    {
        PlayerPrefs.DeleteKey("TutorialCompleted");
        PlayerPrefs.Save();

        Debug.Log("Tutorial reset via Editor!");
        
        EditorUtility.DisplayDialog("Done", "Tutorial has been reset.\n\nIt will appear on next MainMenu load.", "OK");
    }
}
