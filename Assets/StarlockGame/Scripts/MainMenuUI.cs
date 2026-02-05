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

    private void Start()
    {
        SetupButtons();
        ShowMainPanel();
    }

    private void SetupButtons()
    {
        levelsButton.onClick.AddListener(OnLevelsClicked);
        endlessButton.onClick.AddListener(OnEndlessClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);

        levelSelectBackButton.onClick.AddListener(OnLevelSelectBackClicked);
        settingsBackButton.onClick.AddListener(OnSettingsBackClicked);

        if (soundToggle != null && GameManager.Instance != null)
        {
            soundToggle.isOn = GameManager.Instance.SoundEnabled;
            soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
        }

        SetupLevelButtons();
    }

    private void SetupLevelButtons()
    {
        if (levelButtonsContainer == null) return;

        Button[] levelButtons = levelButtonsContainer.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].onClick.AddListener(() => OnLevelSelected(levelIndex));
        }
    }

    private void ShowMainPanel()
    {
        mainPanel.ShowInstant();
        levelSelectPanel.HideInstant();
        settingsPanel.HideInstant();
    }

    private void OnLevelsClicked()
    {
        mainPanel.Hide(() =>
        {
            levelSelectPanel.Show();
        });
    }

    private void OnEndlessClicked()
    {
        GameManager.Instance.StartEndlessMode();
    }

    private void OnSettingsClicked()
    {
        mainPanel.Hide(() =>
        {
            settingsPanel.Show();
        });
    }

    private void OnLevelSelectBackClicked()
    {
        levelSelectPanel.Hide(() =>
        {
            mainPanel.Show();
        });
    }

    private void OnSettingsBackClicked()
    {
        settingsPanel.Hide(() =>
        {
            mainPanel.Show();
        });
    }

    private void OnLevelSelected(int levelIndex)
    {
        GameManager.Instance.StartLevelMode(levelIndex);
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SoundEnabled = isOn;
        }
    }

    private void OnDestroy()
    {
        levelsButton.onClick.RemoveAllListeners();
        endlessButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        levelSelectBackButton.onClick.RemoveAllListeners();
        settingsBackButton.onClick.RemoveAllListeners();

        if (soundToggle != null)
        {
            soundToggle.onValueChanged.RemoveAllListeners();
        }

        if (levelButtonsContainer != null)
        {
            Button[] levelButtons = levelButtonsContainer.GetComponentsInChildren<Button>(true);
            foreach (var btn in levelButtons)
            {
                btn.onClick.RemoveAllListeners();
            }
        }
    }
}
