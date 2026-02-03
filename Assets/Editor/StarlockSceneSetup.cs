using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using TMPro;
using System.IO;

namespace StarlockGame.Editor
{
    public class StarlockSceneSetup : EditorWindow
    {
        private static readonly Color BackgroundColor = new Color(0.08f, 0.08f, 0.12f);
        private static readonly Color PanelColor = new Color(0.12f, 0.12f, 0.18f);
        private static readonly Color PrimaryColor = new Color(0.4f, 0.6f, 1f);
        private static readonly Color SecondaryColor = new Color(0.3f, 0.8f, 0.6f);
        private static readonly Color AccentColor = new Color(1f, 0.6f, 0.3f);
        private static readonly Color TextColor = new Color(0.95f, 0.95f, 0.98f);
        private static readonly Color TextSecondaryColor = new Color(0.6f, 0.6f, 0.7f);
        private static readonly Color ButtonNormalColor = new Color(0.25f, 0.25f, 0.35f);
        private static readonly Color ButtonHoverColor = new Color(0.35f, 0.35f, 0.5f);

        [MenuItem("Starlock/Setup All Scenes")]
        public static void SetupAllScenes()
        {
            if (!EditorUtility.DisplayDialog("Setup Starlock Scenes",
                "This will create MainMenu and Gameplay scenes with full UI setup.\n\nContinue?",
                "Yes", "Cancel"))
            {
                return;
            }

            CreateFolders();
            CreateMainMenuScene();
            CreateGameplayScene();
            AddScenesToBuildSettings();

            EditorUtility.DisplayDialog("Setup Complete",
                "Scenes created successfully!\n\n" +
                "1. Open MainMenu scene\n" +
                "2. Press Play to test",
                "OK");
        }

        [MenuItem("Starlock/Create MainMenu Scene Only")]
        public static void CreateMainMenuSceneOnly()
        {
            CreateFolders();
            CreateMainMenuScene();
            AddScenesToBuildSettings();
        }

        [MenuItem("Starlock/Create Gameplay Scene Only")]
        public static void CreateGameplaySceneOnly()
        {
            CreateFolders();
            CreateGameplayScene();
            AddScenesToBuildSettings();
        }

        private static void CreateFolders()
        {
            string[] folders = new string[]
            {
                "Assets/StarlockGame",
                "Assets/StarlockGame/Scenes",
                "Assets/StarlockGame/Scripts",
                "Assets/StarlockGame/Scripts/Core",
                "Assets/StarlockGame/Scripts/UI",
                "Assets/StarlockGame/Prefabs",
                "Assets/StarlockGame/Audio",
                "Assets/StarlockGame/Sprites"
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

        private static void CreateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera("MainMenu");
            GameObject managers = CreateManagers();
            GameObject sceneTransition = CreateSceneTransition();
            CreateMainMenuUI();

            string scenePath = "Assets/StarlockGame/Scenes/MainMenu.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"MainMenu scene created at: {scenePath}");
        }

        private static void CreateGameplayScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera("Gameplay");
            CreateGameplayUI();

            string scenePath = "Assets/StarlockGame/Scenes/Gameplay.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"Gameplay scene created at: {scenePath}");
        }

        private static void CreateCamera(string sceneName)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            Camera camera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
            
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = BackgroundColor;
            camera.orthographic = true;
            camera.orthographicSize = 5;
            cameraObj.tag = "MainCamera";
        }

        private static GameObject CreateManagers()
        {
            GameObject managersRoot = new GameObject("--- MANAGERS ---");

            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.transform.SetParent(managersRoot.transform);
            gameManagerObj.AddComponent<GameManager>();

            GameObject audioManagerObj = new GameObject("AudioManager");
            audioManagerObj.transform.SetParent(managersRoot.transform);
            audioManagerObj.AddComponent<AudioManager>();

            return managersRoot;
        }

        private static GameObject CreateSceneTransition()
        {
            GameObject transitionRoot = new GameObject("SceneTransition");
            SceneTransition transition = transitionRoot.AddComponent<SceneTransition>();

            GameObject canvasObj = new GameObject("FadeCanvas");
            canvasObj.transform.SetParent(transitionRoot.transform);
            
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject fadeImageObj = new GameObject("FadeImage");
            fadeImageObj.transform.SetParent(canvasObj.transform);
            
            RectTransform fadeRect = fadeImageObj.AddComponent<RectTransform>();
            fadeRect.anchorMin = Vector2.zero;
            fadeRect.anchorMax = Vector2.one;
            fadeRect.offsetMin = Vector2.zero;
            fadeRect.offsetMax = Vector2.zero;
            
            Image fadeImage = fadeImageObj.AddComponent<Image>();
            fadeImage.color = Color.black;
            fadeImage.raycastTarget = true;

            CanvasGroup canvasGroup = fadeImageObj.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;

            SerializedObject so = new SerializedObject(transition);
            so.FindProperty("fadeCanvasGroup").objectReferenceValue = canvasGroup;
            so.FindProperty("fadeImage").objectReferenceValue = fadeImage;
            so.ApplyModifiedProperties();

            return transitionRoot;
        }

        private static void CreateMainMenuUI()
        {
            GameObject canvasObj = CreateUICanvas("UI_Canvas", 100);
            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();

            GameObject background = CreateBackground(canvasObj.transform);

            GameObject mainPanel = CreateMainPanel(canvasObj.transform);
            GameObject levelSelectPanel = CreateLevelSelectPanel(canvasObj.transform);
            GameObject settingsPanel = CreateSettingsPanel(canvasObj.transform);

            levelSelectPanel.SetActive(false);
            settingsPanel.SetActive(false);

            GameObject menuControllerObj = new GameObject("MainMenuController");
            MainMenuUI menuUI = menuControllerObj.AddComponent<MainMenuUI>();

            SerializedObject so = new SerializedObject(menuUI);
            
            so.FindProperty("mainPanel").objectReferenceValue = mainPanel.GetComponent<UIPanel>();
            so.FindProperty("levelSelectPanel").objectReferenceValue = levelSelectPanel.GetComponent<UIPanel>();
            so.FindProperty("settingsPanel").objectReferenceValue = settingsPanel.GetComponent<UIPanel>();

            so.FindProperty("levelsButton").objectReferenceValue = mainPanel.transform.Find("ButtonsContainer/LevelsButton").GetComponent<Button>();
            so.FindProperty("endlessButton").objectReferenceValue = mainPanel.transform.Find("ButtonsContainer/EndlessButton").GetComponent<Button>();
            so.FindProperty("settingsButton").objectReferenceValue = mainPanel.transform.Find("ButtonsContainer/SettingsButton").GetComponent<Button>();

            so.FindProperty("levelSelectBackButton").objectReferenceValue = levelSelectPanel.transform.Find("BackButton").GetComponent<Button>();
            so.FindProperty("levelButtonsContainer").objectReferenceValue = levelSelectPanel.transform.Find("LevelButtonsContainer/Viewport/Content");

            so.FindProperty("settingsBackButton").objectReferenceValue = settingsPanel.transform.Find("BackButton").GetComponent<Button>();
            so.FindProperty("soundToggle").objectReferenceValue = settingsPanel.transform.Find("SoundToggle").GetComponent<Toggle>();

            so.ApplyModifiedProperties();
        }

        private static GameObject CreateBackground(Transform parent)
        {
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(parent);
            
            RectTransform rect = bg.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            Image image = bg.AddComponent<Image>();
            image.color = BackgroundColor;
            image.raycastTarget = false;

            bg.transform.SetAsFirstSibling();
            return bg;
        }

        private static GameObject CreateMainPanel(Transform parent)
        {
            GameObject panel = new GameObject("MainPanel");
            panel.transform.SetParent(parent);

            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            CanvasGroup cg = panel.AddComponent<CanvasGroup>();
            UIPanel uiPanel = panel.AddComponent<UIPanel>();

            SerializedObject so = new SerializedObject(uiPanel);
            so.FindProperty("useScaleAnimation").boolValue = false;
            so.FindProperty("useFadeAnimation").boolValue = true;
            so.ApplyModifiedProperties();

            GameObject titleContainer = new GameObject("TitleContainer");
            titleContainer.transform.SetParent(panel.transform);
            RectTransform titleContainerRect = titleContainer.AddComponent<RectTransform>();
            titleContainerRect.anchorMin = new Vector2(0, 0.55f);
            titleContainerRect.anchorMax = new Vector2(1, 0.9f);
            titleContainerRect.offsetMin = new Vector2(60, 0);
            titleContainerRect.offsetMax = new Vector2(-60, 0);

            GameObject title = CreateText("Title", titleContainer.transform, "STARLOCK", 96, FontStyles.Bold, TextColor);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.5f);
            titleRect.anchorMax = new Vector2(1, 1f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject subtitle = CreateText("Subtitle", titleContainer.transform, "TAP • MATCH • CLEAR", 28, FontStyles.Normal, TextSecondaryColor);
            RectTransform subtitleRect = subtitle.GetComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0, 0);
            subtitleRect.anchorMax = new Vector2(1, 0.45f);
            subtitleRect.offsetMin = Vector2.zero;
            subtitleRect.offsetMax = Vector2.zero;
            subtitle.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject buttonsContainer = new GameObject("ButtonsContainer");
            buttonsContainer.transform.SetParent(panel.transform);
            RectTransform buttonsRect = buttonsContainer.AddComponent<RectTransform>();
            buttonsRect.anchorMin = new Vector2(0.5f, 0.15f);
            buttonsRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonsRect.sizeDelta = new Vector2(500, 400);
            buttonsRect.anchoredPosition = Vector2.zero;

            VerticalLayoutGroup layout = buttonsContainer.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 30;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(20, 20, 20, 20);

            CreateMenuButton("LevelsButton", buttonsContainer.transform, "LEVELS", PrimaryColor, 100);
            CreateMenuButton("EndlessButton", buttonsContainer.transform, "ENDLESS", SecondaryColor, 100);
            CreateMenuButton("SettingsButton", buttonsContainer.transform, "SETTINGS", ButtonNormalColor, 80);

            return panel;
        }

        private static GameObject CreateLevelSelectPanel(Transform parent)
        {
            GameObject panel = new GameObject("LevelSelectPanel");
            panel.transform.SetParent(parent);

            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            CanvasGroup cg = panel.AddComponent<CanvasGroup>();
            UIPanel uiPanel = panel.AddComponent<UIPanel>();

            SerializedObject so = new SerializedObject(uiPanel);
            so.FindProperty("useScaleAnimation").boolValue = true;
            so.FindProperty("useFadeAnimation").boolValue = true;
            so.FindProperty("startScale").floatValue = 0.9f;
            so.ApplyModifiedProperties();

            GameObject panelBg = new GameObject("PanelBackground");
            panelBg.transform.SetParent(panel.transform);
            RectTransform bgRect = panelBg.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.05f, 0.08f);
            bgRect.anchorMax = new Vector2(0.95f, 0.92f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            Image bgImage = panelBg.AddComponent<Image>();
            bgImage.color = PanelColor;
            bgImage.raycastTarget = true;
            AddRoundedCorners(panelBg, 40);

            GameObject header = CreateText("Header", panel.transform, "SELECT LEVEL", 48, FontStyles.Bold, TextColor);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.85f);
            headerRect.anchorMax = new Vector2(1, 0.92f);
            headerRect.offsetMin = new Vector2(40, 0);
            headerRect.offsetMax = new Vector2(-40, 0);
            header.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject scrollView = CreateScrollView(panel.transform, "LevelButtonsContainer");
            RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0.1f, 0.18f);
            scrollRect.anchorMax = new Vector2(0.9f, 0.82f);
            scrollRect.offsetMin = Vector2.zero;
            scrollRect.offsetMax = Vector2.zero;

            Transform content = scrollView.transform.Find("Viewport/Content");
            GridLayoutGroup grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(180, 180);
            grid.spacing = new Vector2(30, 30);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;
            grid.padding = new RectOffset(20, 20, 20, 20);

            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            for (int i = 1; i <= 10; i++)
            {
                CreateLevelButton(content, i);
            }

            GameObject backButton = CreateBackButton(panel.transform);

            return panel;
        }

        private static GameObject CreateSettingsPanel(Transform parent)
        {
            GameObject panel = new GameObject("SettingsPanel");
            panel.transform.SetParent(parent);

            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            CanvasGroup cg = panel.AddComponent<CanvasGroup>();
            UIPanel uiPanel = panel.AddComponent<UIPanel>();

            SerializedObject so = new SerializedObject(uiPanel);
            so.FindProperty("useScaleAnimation").boolValue = true;
            so.FindProperty("useFadeAnimation").boolValue = true;
            so.FindProperty("startScale").floatValue = 0.9f;
            so.ApplyModifiedProperties();

            GameObject panelBg = new GameObject("PanelBackground");
            panelBg.transform.SetParent(panel.transform);
            RectTransform bgRect = panelBg.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.1f, 0.3f);
            bgRect.anchorMax = new Vector2(0.9f, 0.75f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            Image bgImage = panelBg.AddComponent<Image>();
            bgImage.color = PanelColor;
            bgImage.raycastTarget = true;
            AddRoundedCorners(panelBg, 40);

            GameObject header = CreateText("Header", panel.transform, "SETTINGS", 48, FontStyles.Bold, TextColor);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.65f);
            headerRect.anchorMax = new Vector2(1, 0.73f);
            headerRect.offsetMin = new Vector2(40, 0);
            headerRect.offsetMax = new Vector2(-40, 0);
            header.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject soundToggle = CreateToggle(panel.transform, "SoundToggle", "SOUND", true);
            RectTransform toggleRect = soundToggle.GetComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0.5f, 0.45f);
            toggleRect.anchorMax = new Vector2(0.5f, 0.55f);
            toggleRect.sizeDelta = new Vector2(400, 80);
            toggleRect.anchoredPosition = Vector2.zero;

            GameObject backButton = CreateBackButton(panel.transform);

            return panel;
        }

        private static GameObject CreateScrollView(Transform parent, string name)
        {
            GameObject scrollView = new GameObject(name);
            scrollView.transform.SetParent(parent);

            RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.elasticity = 0.1f;
            scroll.inertia = true;
            scroll.decelerationRate = 0.135f;
            scroll.scrollSensitivity = 20f;

            Image scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0, 0, 0, 0);

            Mask mask = scrollView.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = new Color(0, 0, 0, 0);

            Mask viewportMask = viewport.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;

            scroll.viewport = viewportRect;
            scroll.content = contentRect;

            return scrollView;
        }

        private static GameObject CreateMenuButton(string name, Transform parent, string text, Color bgColor, float height)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, height);

            Image image = buttonObj.AddComponent<Image>();
            image.color = bgColor;
            AddRoundedCorners(buttonObj, 20);

            Button button = buttonObj.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f);
            colors.pressedColor = new Color(0.9f, 0.9f, 0.9f);
            colors.selectedColor = Color.white;
            button.colors = colors;

            buttonObj.AddComponent<AnimatedButton>();

            GameObject textObj = CreateText("Text", buttonObj.transform, text, 36, FontStyles.Bold, TextColor);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(20, 0);
            textRect.offsetMax = new Vector2(-20, 0);
            textObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            LayoutElement layoutElement = buttonObj.AddComponent<LayoutElement>();
            layoutElement.minHeight = height;
            layoutElement.preferredHeight = height;

            return buttonObj;
        }

        private static GameObject CreateLevelButton(Transform parent, int levelNumber)
        {
            GameObject buttonObj = new GameObject($"Level_{levelNumber}");
            buttonObj.transform.SetParent(parent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();

            Image image = buttonObj.AddComponent<Image>();
            image.color = ButtonNormalColor;
            AddRoundedCorners(buttonObj, 25);

            Button button = buttonObj.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.15f, 1.15f, 1.15f);
            colors.pressedColor = new Color(0.85f, 0.85f, 0.85f);
            button.colors = colors;

            buttonObj.AddComponent<AnimatedButton>();

            GameObject textObj = CreateText("Text", buttonObj.transform, levelNumber.ToString(), 48, FontStyles.Bold, TextColor);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            textObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            return buttonObj;
        }

        private static GameObject CreateBackButton(Transform parent)
        {
            GameObject buttonObj = new GameObject("BackButton");
            buttonObj.transform.SetParent(parent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.02f);
            rect.anchorMax = new Vector2(0.5f, 0.02f);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(200, 70);
            rect.anchoredPosition = new Vector2(0, 60);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.4f);
            AddRoundedCorners(buttonObj, 15);

            Button button = buttonObj.AddComponent<Button>();
            buttonObj.AddComponent<AnimatedButton>();

            GameObject textObj = CreateText("Text", buttonObj.transform, "← BACK", 28, FontStyles.Bold, TextColor);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            textObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            return buttonObj;
        }

        private static GameObject CreateToggle(Transform parent, string name, string labelText, bool defaultValue)
        {
            GameObject toggleObj = new GameObject(name);
            toggleObj.transform.SetParent(parent);

            RectTransform rect = toggleObj.AddComponent<RectTransform>();

            Toggle toggle = toggleObj.AddComponent<Toggle>();
            toggle.isOn = defaultValue;

            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(toggleObj.transform);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(1, 0.5f);
            bgRect.anchorMax = new Vector2(1, 0.5f);
            bgRect.pivot = new Vector2(1, 0.5f);
            bgRect.sizeDelta = new Vector2(80, 45);
            bgRect.anchoredPosition = new Vector2(0, 0);

            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.3f);
            AddRoundedCorners(bg, 22);

            GameObject checkmark = new GameObject("Checkmark");
            checkmark.transform.SetParent(bg.transform);
            RectTransform checkRect = checkmark.AddComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkRect.sizeDelta = new Vector2(35, 35);
            checkRect.anchoredPosition = new Vector2(15, 0);

            Image checkImage = checkmark.AddComponent<Image>();
            checkImage.color = SecondaryColor;
            AddRoundedCorners(checkmark, 17);

            toggle.targetGraphic = bgImage;
            toggle.graphic = checkImage;

            ColorBlock colors = toggle.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f);
            toggle.colors = colors;

            GameObject label = CreateText("Label", toggleObj.transform, labelText, 32, FontStyles.Normal, TextColor);
            RectTransform labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(0.6f, 1);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            label.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

            return toggleObj;
        }

        private static void CreateGameplayUI()
        {
            GameObject canvasObj = CreateUICanvas("UI_Canvas", 100);

            GameObject background = CreateBackground(canvasObj.transform);

            GameObject header = new GameObject("Header");
            header.transform.SetParent(canvasObj.transform);
            RectTransform headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.9f);
            headerRect.anchorMax = new Vector2(1, 1f);
            headerRect.offsetMin = new Vector2(30, 0);
            headerRect.offsetMax = new Vector2(-30, -30);

            Image headerBg = header.AddComponent<Image>();
            headerBg.color = new Color(0, 0, 0, 0.3f);
            AddRoundedCorners(header, 15);

            GameObject levelText = CreateText("LevelText", header.transform, "LEVEL 1", 32, FontStyles.Bold, TextColor);
            RectTransform levelRect = levelText.GetComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(0, 0);
            levelRect.anchorMax = new Vector2(0.5f, 1);
            levelRect.offsetMin = new Vector2(20, 0);
            levelRect.offsetMax = Vector2.zero;
            levelText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

            GameObject scoreText = CreateText("ScoreText", header.transform, "0", 42, FontStyles.Bold, SecondaryColor);
            RectTransform scoreRect = scoreText.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0.5f, 0);
            scoreRect.anchorMax = new Vector2(1, 1);
            scoreRect.offsetMin = Vector2.zero;
            scoreRect.offsetMax = new Vector2(-20, 0);
            scoreText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;

            GameObject debugPanel = new GameObject("DebugPanel");
            debugPanel.transform.SetParent(canvasObj.transform);
            RectTransform debugRect = debugPanel.AddComponent<RectTransform>();
            debugRect.anchorMin = new Vector2(0, 0);
            debugRect.anchorMax = new Vector2(1, 0.12f);
            debugRect.offsetMin = new Vector2(30, 30);
            debugRect.offsetMax = new Vector2(-30, 0);

            Image debugBg = debugPanel.AddComponent<Image>();
            debugBg.color = new Color(0, 0, 0, 0.5f);
            AddRoundedCorners(debugPanel, 15);

            GameObject modeText = CreateText("ModeText", debugPanel.transform, "Level 1", 24, FontStyles.Normal, TextSecondaryColor);
            RectTransform modeRect = modeText.GetComponent<RectTransform>();
            modeRect.anchorMin = new Vector2(0, 0.5f);
            modeRect.anchorMax = new Vector2(0.5f, 1);
            modeRect.offsetMin = new Vector2(20, 0);
            modeRect.offsetMax = Vector2.zero;
            modeText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

            GameObject backButton = new GameObject("BackToMenuButton");
            backButton.transform.SetParent(debugPanel.transform);
            RectTransform backRect = backButton.AddComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0.6f, 0.15f);
            backRect.anchorMax = new Vector2(0.98f, 0.85f);
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = Vector2.zero;

            Image backImage = backButton.AddComponent<Image>();
            backImage.color = AccentColor;
            AddRoundedCorners(backButton, 12);

            Button backBtn = backButton.AddComponent<Button>();
            backButton.AddComponent<AnimatedButton>();

            GameObject backText = CreateText("Text", backButton.transform, "← MENU", 22, FontStyles.Bold, TextColor);
            RectTransform backTextRect = backText.GetComponent<RectTransform>();
            backTextRect.anchorMin = Vector2.zero;
            backTextRect.anchorMax = Vector2.one;
            backTextRect.offsetMin = Vector2.zero;
            backTextRect.offsetMax = Vector2.zero;
            backText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject gameAreaPlaceholder = CreateText("GameAreaPlaceholder", canvasObj.transform, 
                "GAME AREA\n(Circle & Shapes will be here)", 36, FontStyles.Normal, TextSecondaryColor);
            RectTransform placeholderRect = gameAreaPlaceholder.GetComponent<RectTransform>();
            placeholderRect.anchorMin = new Vector2(0, 0.2f);
            placeholderRect.anchorMax = new Vector2(1, 0.85f);
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;
            gameAreaPlaceholder.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject controllerObj = new GameObject("GameplayController");
            GameplayUI gameplayUI = controllerObj.AddComponent<GameplayUI>();

            SerializedObject so = new SerializedObject(gameplayUI);
            so.FindProperty("scoreText").objectReferenceValue = scoreText.GetComponent<TextMeshProUGUI>();
            so.FindProperty("levelText").objectReferenceValue = levelText.GetComponent<TextMeshProUGUI>();
            so.FindProperty("modeText").objectReferenceValue = modeText.GetComponent<TextMeshProUGUI>();
            so.FindProperty("backToMenuButton").objectReferenceValue = backBtn;
            so.ApplyModifiedProperties();
        }

        private static GameObject CreateUICanvas(string name, int sortOrder)
        {
            GameObject canvasObj = new GameObject(name);
            
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();

            return canvasObj;
        }

        private static GameObject CreateText(string name, Transform parent, string text, float fontSize, FontStyles style, Color color)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent);

            RectTransform rect = textObj.AddComponent<RectTransform>();

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;

            return textObj;
        }

        private static void AddRoundedCorners(GameObject obj, float radius)
        {
        }

        private static void AddScenesToBuildSettings()
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            
            string mainMenuPath = "Assets/StarlockGame/Scenes/MainMenu.unity";
            string gameplayPath = "Assets/StarlockGame/Scenes/Gameplay.unity";

            bool hasMainMenu = false;
            bool hasGameplay = false;

            foreach (var scene in scenes)
            {
                if (scene.path == mainMenuPath) hasMainMenu = true;
                if (scene.path == gameplayPath) hasGameplay = true;
            }

            if (!hasMainMenu && System.IO.File.Exists(mainMenuPath))
            {
                scenes.Insert(0, new EditorBuildSettingsScene(mainMenuPath, true));
            }

            if (!hasGameplay && System.IO.File.Exists(gameplayPath))
            {
                scenes.Add(new EditorBuildSettingsScene(gameplayPath, true));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("Build Settings updated with Starlock scenes");
        }
    }
}
