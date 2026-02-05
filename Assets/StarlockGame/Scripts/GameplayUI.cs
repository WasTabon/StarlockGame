using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button pauseButton;

    [Header("Popups")]
    [SerializeField] private VictoryPopup victoryPopup;
    [SerializeField] private GameOverPopup gameOverPopup;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private Button backToMenuButton;

    public System.Action OnRestartClicked;
    public System.Action OnNextLevelClicked;
    public System.Action OnMenuClicked;

    private void Start()
    {
        UpdateModeDisplay();
        SetupButtons();
        HideAllPopups();
    }

    private void SetupButtons()
    {
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
        }

        if (victoryPopup != null)
        {
            victoryPopup.OnNextLevelClicked += () => OnNextLevelClicked?.Invoke();
            victoryPopup.OnRestartClicked += () => OnRestartClicked?.Invoke();
            victoryPopup.OnMenuClicked += () => OnMenuClicked?.Invoke();
        }

        if (gameOverPopup != null)
        {
            gameOverPopup.OnRestartClicked += () => OnRestartClicked?.Invoke();
            gameOverPopup.OnMenuClicked += () => OnMenuClicked?.Invoke();
        }
    }

    private void UpdateModeDisplay()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager not found!");
            return;
        }

        if (modeText != null)
        {
            if (GameManager.Instance.CurrentMode == GameMode.Levels)
            {
                modeText.text = $"Level {GameManager.Instance.SelectedLevel}";
            }
            else if (GameManager.Instance.CurrentMode == GameMode.Endless)
            {
                modeText.text = "Endless Mode";
            }
            else
            {
                modeText.text = "Unknown Mode";
            }
        }

        if (levelText != null)
        {
            if (GameManager.Instance.CurrentMode == GameMode.Levels)
            {
                levelText.text = $"LEVEL {GameManager.Instance.SelectedLevel}";
            }
            else
            {
                levelText.text = "ENDLESS";
            }
        }

        if (scoreText != null)
        {
            scoreText.text = "0";
        }
    }

    private void OnBackToMenuClicked()
    {
        OnMenuClicked?.Invoke();
    }

    private void OnPauseClicked()
    {
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void ShowVictoryPopup(int score, bool hasNextLevel)
    {
        if (victoryPopup == null)
        {
            Debug.LogWarning("VictoryPopup not assigned!");
            return;
        }

        victoryPopup.Setup(score, hasNextLevel);
        victoryPopup.Show();
    }

    public void ShowGameOverPopup(int score)
    {
        if (gameOverPopup == null)
        {
            Debug.LogWarning("GameOverPopup not assigned!");
            return;
        }

        gameOverPopup.Setup(score);
        gameOverPopup.Show();
    }

    public void HideAllPopups()
    {
        if (victoryPopup != null)
        {
            victoryPopup.HideInstant();
        }

        if (gameOverPopup != null)
        {
            gameOverPopup.HideInstant();
        }
    }

    private void OnDestroy()
    {
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.RemoveAllListeners();
        }
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
        }
    }
}
