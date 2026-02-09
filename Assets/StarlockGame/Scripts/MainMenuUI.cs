using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private UIPanel mainPanel;
    [SerializeField] private UIPanel levelSelectPanel;
    [SerializeField] private UIPanel settingsPanel;

    [Header("Main Panel Buttons")]
    [SerializeField] private Button levelsButton;
    [SerializeField] private Button endlessButton;
    [SerializeField] private Button settingsButton;

    [Header("Level Select")]
    [SerializeField] private Button levelSelectBackButton;
    [SerializeField] private Transform levelButtonsContainer;

    [Header("Settings")]
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Button resetProgressButton;

    [Header("Tutorial")]
    [SerializeField] private TutorialManager tutorialManager;

    private LevelButton[] levelButtons;

    private void Start()
    {
        SetupButtons();
        SetupLevelButtons();
        
        CheckTutorial();
    }

    private void CheckTutorial()
    {
        if (tutorialManager != null && tutorialManager.ShouldShowTutorial())
        {
            if (mainPanel != null)
            {
                mainPanel.HideInstant();
            }

            tutorialManager.StartTutorial(() =>
            {
                ShowMainPanel();
            });
        }
        else
        {
            ShowMainPanel();
        }
    }

    private void SetupButtons()
    {
        if (levelsButton != null)
            levelsButton.onClick.AddListener(OnLevelsClicked);
        
        if (endlessButton != null)
            endlessButton.onClick.AddListener(OnEndlessClicked);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (levelSelectBackButton != null)
            levelSelectBackButton.onClick.AddListener(OnLevelSelectBackClicked);
        
        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(OnSettingsBackClicked);

        if (soundToggle != null && GameManager.Instance != null)
        {
            soundToggle.isOn = GameManager.Instance.SoundEnabled;
            soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
        }

        if (resetProgressButton != null)
        {
            resetProgressButton.onClick.AddListener(OnResetProgressClicked);
        }
    }

    private void SetupLevelButtons()
    {
        if (levelButtonsContainer == null) return;

        levelButtons = levelButtonsContainer.GetComponentsInChildren<LevelButton>(true);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].Setup(levelIndex);
            levelButtons[i].OnLevelSelected += OnLevelSelected;
        }

        Button[] oldButtons = levelButtonsContainer.GetComponentsInChildren<Button>(true);
        foreach (Button btn in oldButtons)
        {
            if (btn.GetComponent<LevelButton>() == null)
            {
                int siblingIndex = btn.transform.GetSiblingIndex();
                int levelIndex = siblingIndex + 1;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnLevelSelected(levelIndex));
            }
        }
    }

    private void RefreshLevelButtons()
    {
        if (levelButtons == null) return;

        foreach (LevelButton levelButton in levelButtons)
        {
            if (levelButton != null)
            {
                levelButton.UpdateVisuals();
            }
        }
    }

    private void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.ShowInstant();
        if (levelSelectPanel != null) levelSelectPanel.HideInstant();
        if (settingsPanel != null) settingsPanel.HideInstant();
    }

    private void OnLevelsClicked()
    {
        RefreshLevelButtons();

        if (mainPanel != null)
        {
            mainPanel.Hide(() =>
            {
                if (levelSelectPanel != null)
                    levelSelectPanel.Show();
            });
        }
    }

    private void OnEndlessClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartEndlessMode();
    }

    private void OnSettingsClicked()
    {
        if (mainPanel != null)
        {
            mainPanel.Hide(() =>
            {
                if (settingsPanel != null)
                    settingsPanel.Show();
            });
        }
    }

    private void OnLevelSelectBackClicked()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.Hide(() =>
            {
                if (mainPanel != null)
                    mainPanel.Show();
            });
        }
    }

    private void OnSettingsBackClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.Hide(() =>
            {
                if (mainPanel != null)
                    mainPanel.Show();
            });
        }
    }

    private void OnLevelSelected(int levelIndex)
    {
        if (ProgressManager.Instance != null && !ProgressManager.Instance.IsLevelUnlocked(levelIndex))
        {
            Debug.Log($"Level {levelIndex} is locked!");
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.StartLevelMode(levelIndex);
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SoundEnabled = isOn;
        }
    }

    private void OnResetProgressClicked()
    {
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.ResetAllProgress();
            RefreshLevelButtons();
        }
    }

    private void OnDestroy()
    {
        if (levelsButton != null)
            levelsButton.onClick.RemoveAllListeners();
        
        if (endlessButton != null)
            endlessButton.onClick.RemoveAllListeners();
        
        if (settingsButton != null)
            settingsButton.onClick.RemoveAllListeners();
        
        if (levelSelectBackButton != null)
            levelSelectBackButton.onClick.RemoveAllListeners();
        
        if (settingsBackButton != null)
            settingsBackButton.onClick.RemoveAllListeners();

        if (soundToggle != null)
            soundToggle.onValueChanged.RemoveAllListeners();

        if (resetProgressButton != null)
            resetProgressButton.onClick.RemoveAllListeners();

        if (levelButtons != null)
        {
            foreach (LevelButton levelButton in levelButtons)
            {
                if (levelButton != null)
                    levelButton.OnLevelSelected -= OnLevelSelected;
            }
        }
    }
}
