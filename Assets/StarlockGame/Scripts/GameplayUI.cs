using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button pauseButton;

    [Header("Endless HUD")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private GameObject endlessHUD;

    [Header("Popups")]
    [SerializeField] private VictoryPopup victoryPopup;
    [SerializeField] private GameOverPopup gameOverPopup;
    [SerializeField] private PausePopup pausePopup;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private Button backToMenuButton;

    public System.Action OnRestartClicked;
    public System.Action OnNextLevelClicked;
    public System.Action OnMenuClicked;
    public System.Action OnPauseClicked;
    public System.Action OnResumeClicked;

    private bool isEndlessMode = false;
    private bool isNewHighscore = false;

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
            backToMenuButton.onClick.RemoveAllListeners();
            backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        if (victoryPopup != null)
        {
            victoryPopup.OnNextLevelClicked = null;
            victoryPopup.OnRestartClicked = null;
            victoryPopup.OnMenuClicked = null;
            
            victoryPopup.OnNextLevelClicked += () => OnNextLevelClicked?.Invoke();
            victoryPopup.OnRestartClicked += () => OnRestartClicked?.Invoke();
            victoryPopup.OnMenuClicked += () => OnMenuClicked?.Invoke();
        }

        if (gameOverPopup != null)
        {
            gameOverPopup.OnRestartClicked = null;
            gameOverPopup.OnMenuClicked = null;
            
            gameOverPopup.OnRestartClicked += () => OnRestartClicked?.Invoke();
            gameOverPopup.OnMenuClicked += () => OnMenuClicked?.Invoke();
        }

        if (pausePopup != null)
        {
            pausePopup.OnResumeClicked = null;
            pausePopup.OnRestartClicked = null;
            pausePopup.OnMenuClicked = null;
            
            pausePopup.OnResumeClicked += () => OnResumeClicked?.Invoke();
            pausePopup.OnRestartClicked += () => OnRestartClicked?.Invoke();
            pausePopup.OnMenuClicked += () => OnMenuClicked?.Invoke();
        }
    }

    private void OnPauseButtonClicked()
    {
        Debug.Log("Pause button clicked");
        OnPauseClicked?.Invoke();
    }

    private void UpdateModeDisplay()
    {
        if (GameManager.Instance == null)
        {
            if (levelText != null) levelText.text = "LEVEL 1";
            if (scoreText != null) scoreText.text = "0";
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

        UpdateHighscoreDisplay();
    }

    private void UpdateHighscoreDisplay()
    {
        if (highscoreText == null) return;

        if (HighscoreManager.Instance != null)
        {
            int highscore = HighscoreManager.Instance.EndlessHighscore;
            highscoreText.text = $"BEST: {highscore}";
        }
        else
        {
            highscoreText.text = "BEST: 0";
        }
    }

    private void OnBackToMenuClicked()
    {
        OnMenuClicked?.Invoke();
    }

    public void SetEndlessMode(bool endless)
    {
        isEndlessMode = endless;

        if (endlessHUD != null)
        {
            endlessHUD.SetActive(endless);
        }

        if (timeText != null)
        {
            timeText.gameObject.SetActive(endless);
        }

        if (highscoreText != null)
        {
            highscoreText.gameObject.SetActive(endless);
        }

        UpdateHighscoreDisplay();
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void UpdateTime(float time)
    {
        if (timeText == null) return;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    public void SetNewHighscore(bool isNew)
    {
        isNewHighscore = isNew;
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

    public void ShowEndlessGameOverPopup(int score, int highscore, float time)
    {
        if (gameOverPopup == null)
        {
            Debug.LogWarning("GameOverPopup not assigned!");
            return;
        }

        gameOverPopup.SetupEndless(score, highscore, time, isNewHighscore);
        gameOverPopup.Show();
    }

    public void ShowPausePopup()
    {
        if (pausePopup == null)
        {
            Debug.LogWarning("PausePopup not assigned!");
            return;
        }

        pausePopup.ShowInstant();
    }

    public void HidePausePopup()
    {
        if (pausePopup != null)
        {
            pausePopup.HideInstant();
        }
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

        if (pausePopup != null)
        {
            pausePopup.HideInstant();
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
