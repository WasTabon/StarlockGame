using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StarlockSetup_06_Popups : EditorWindow
{
    private static readonly Color PanelColor = new Color(0.12f, 0.12f, 0.18f, 0.95f);
    private static readonly Color ButtonGreen = new Color(0.3f, 0.8f, 0.5f);
    private static readonly Color ButtonOrange = new Color(1f, 0.6f, 0.3f);
    private static readonly Color ButtonGray = new Color(0.4f, 0.4f, 0.5f);
    private static readonly Color TextColor = new Color(0.95f, 0.95f, 0.98f);

    [MenuItem("Starlock/Setup Popups (Iteration 6)")]
    public static void SetupPopups()
    {
        if (!EditorUtility.DisplayDialog("Setup Popups",
            "This will:\n" +
            "- Create Victory popup\n" +
            "- Create Game Over popup\n" +
            "- Update GameplayUI references\n\n" +
            "Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        Canvas canvas = FindUICanvas();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "UI_Canvas not found!\n\nMake sure you have Gameplay scene open.", "OK");
            return;
        }

        CreateVictoryPopup(canvas.transform);
        CreateGameOverPopup(canvas.transform);
        UpdateGameplayUI();
        UpdateGameplayController();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Setup Complete",
            "Popups created!\n\n" +
            "- VictoryPopup added\n" +
            "- GameOverPopup added\n" +
            "- References configured\n\n" +
            "Press Play to test!",
            "OK");
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

    private static void CreateVictoryPopup(Transform parent)
    {
        GameObject existing = GameObject.Find("VictoryPopup");
        if (existing != null)
        {
            Debug.Log("VictoryPopup already exists, skipping...");
            return;
        }

        GameObject popup = CreatePopupBase(parent, "VictoryPopup");

        VictoryPopup victoryComp = popup.AddComponent<VictoryPopup>();

        Transform panelBg = popup.transform.Find("PanelBackground");

        GameObject title = CreateText("Title", panelBg, "VICTORY!", 64, FontStyles.Bold, TextColor);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.7f);
        titleRect.anchorMax = new Vector2(1, 0.9f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        GameObject score = CreateText("ScoreText", panelBg, "Score: 0", 36, FontStyles.Normal, TextColor);
        RectTransform scoreRect = score.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 0.55f);
        scoreRect.anchorMax = new Vector2(1, 0.7f);
        scoreRect.offsetMin = Vector2.zero;
        scoreRect.offsetMax = Vector2.zero;

        GameObject nextLevelBtn = CreateButton("NextLevelButton", panelBg, "NEXT LEVEL", ButtonGreen);
        RectTransform nextRect = nextLevelBtn.GetComponent<RectTransform>();
        nextRect.anchorMin = new Vector2(0.1f, 0.35f);
        nextRect.anchorMax = new Vector2(0.9f, 0.5f);
        nextRect.offsetMin = Vector2.zero;
        nextRect.offsetMax = Vector2.zero;

        GameObject restartBtn = CreateButton("RestartButton", panelBg, "RESTART", ButtonOrange);
        RectTransform restartRect = restartBtn.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.1f, 0.2f);
        restartRect.anchorMax = new Vector2(0.9f, 0.32f);
        restartRect.offsetMin = Vector2.zero;
        restartRect.offsetMax = Vector2.zero;

        GameObject menuBtn = CreateButton("MenuButton", panelBg, "MENU", ButtonGray);
        RectTransform menuRect = menuBtn.GetComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0.1f, 0.05f);
        menuRect.anchorMax = new Vector2(0.9f, 0.17f);
        menuRect.offsetMin = Vector2.zero;
        menuRect.offsetMax = Vector2.zero;

        SerializedObject so = new SerializedObject(victoryComp);
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("scoreText").objectReferenceValue = score.GetComponent<TextMeshProUGUI>();
        so.FindProperty("nextLevelButton").objectReferenceValue = nextLevelBtn.GetComponent<Button>();
        so.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
        so.FindProperty("menuButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        popup.SetActive(false);

        Debug.Log("VictoryPopup created");
    }

    private static void CreateGameOverPopup(Transform parent)
    {
        GameObject existing = GameObject.Find("GameOverPopup");
        if (existing != null)
        {
            Debug.Log("GameOverPopup already exists, skipping...");
            return;
        }

        GameObject popup = CreatePopupBase(parent, "GameOverPopup");

        GameOverPopup gameOverComp = popup.AddComponent<GameOverPopup>();

        Transform panelBg = popup.transform.Find("PanelBackground");

        GameObject title = CreateText("Title", panelBg, "GAME OVER", 64, FontStyles.Bold, new Color(1f, 0.4f, 0.4f));
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.7f);
        titleRect.anchorMax = new Vector2(1, 0.9f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        GameObject score = CreateText("ScoreText", panelBg, "Score: 0", 36, FontStyles.Normal, TextColor);
        RectTransform scoreRect = score.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 0.55f);
        scoreRect.anchorMax = new Vector2(1, 0.7f);
        scoreRect.offsetMin = Vector2.zero;
        scoreRect.offsetMax = Vector2.zero;

        GameObject restartBtn = CreateButton("RestartButton", panelBg, "TRY AGAIN", ButtonOrange);
        RectTransform restartRect = restartBtn.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.1f, 0.3f);
        restartRect.anchorMax = new Vector2(0.9f, 0.45f);
        restartRect.offsetMin = Vector2.zero;
        restartRect.offsetMax = Vector2.zero;

        GameObject menuBtn = CreateButton("MenuButton", panelBg, "MENU", ButtonGray);
        RectTransform menuRect = menuBtn.GetComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0.1f, 0.1f);
        menuRect.anchorMax = new Vector2(0.9f, 0.25f);
        menuRect.offsetMin = Vector2.zero;
        menuRect.offsetMax = Vector2.zero;

        SerializedObject so = new SerializedObject(gameOverComp);
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("scoreText").objectReferenceValue = score.GetComponent<TextMeshProUGUI>();
        so.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
        so.FindProperty("menuButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        popup.SetActive(false);

        Debug.Log("GameOverPopup created");
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

        popup.AddComponent<CanvasGroup>();

        GameObject dimmer = new GameObject("Dimmer");
        dimmer.transform.SetParent(popup.transform);
        RectTransform dimmerRect = dimmer.AddComponent<RectTransform>();
        dimmerRect.anchorMin = Vector2.zero;
        dimmerRect.anchorMax = Vector2.one;
        dimmerRect.offsetMin = Vector2.zero;
        dimmerRect.offsetMax = Vector2.zero;

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

        Image bgImage = panelBg.AddComponent<Image>();
        bgImage.color = PanelColor;
        bgImage.raycastTarget = true;

        return popup;
    }

    private static GameObject CreateText(string name, Transform parent, string text, float fontSize, FontStyles style, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        textObj.AddComponent<RectTransform>();

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
        buttonObj.AddComponent<RectTransform>();

        Image image = buttonObj.AddComponent<Image>();
        image.color = bgColor;

        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f);
        colors.pressedColor = new Color(0.9f, 0.9f, 0.9f);
        button.colors = colors;

        buttonObj.AddComponent<AnimatedButton>();

        GameObject textObj = CreateText("Text", buttonObj.transform, text, 32, FontStyles.Bold, TextColor);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
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

        VictoryPopup victoryPopup = Object.FindFirstObjectByType<VictoryPopup>(FindObjectsInactive.Include);
        if (victoryPopup != null)
        {
            so.FindProperty("victoryPopup").objectReferenceValue = victoryPopup;
        }

        GameOverPopup gameOverPopup = Object.FindFirstObjectByType<GameOverPopup>(FindObjectsInactive.Include);
        if (gameOverPopup != null)
        {
            so.FindProperty("gameOverPopup").objectReferenceValue = gameOverPopup;
        }

        so.ApplyModifiedProperties();

        Debug.Log("GameplayUI references updated");
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

        GameplayUI gameplayUI = Object.FindFirstObjectByType<GameplayUI>();
        if (gameplayUI != null)
        {
            so.FindProperty("gameplayUI").objectReferenceValue = gameplayUI;
        }

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

        so.ApplyModifiedProperties();

        Debug.Log("GameplayController references updated");
    }
}
