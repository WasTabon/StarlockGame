using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPopup : UIPanel
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI newHighscoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    public System.Action OnRestartClicked;
    public System.Action OnMenuClicked;

    private bool buttonsSetup = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (buttonsSetup) return;

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
        }

        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());
        }

        buttonsSetup = true;
    }

    public void Setup(int score)
    {
        SetupButtons();

        if (titleText != null)
        {
            titleText.text = "GAME OVER";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        if (highscoreText != null)
        {
            highscoreText.gameObject.SetActive(false);
        }

        if (timeText != null)
        {
            timeText.gameObject.SetActive(false);
        }

        if (newHighscoreText != null)
        {
            newHighscoreText.gameObject.SetActive(false);
        }
    }

    public void SetupEndless(int score, int highscore, float time, bool isNewHighscore)
    {
        SetupButtons();

        if (titleText != null)
        {
            titleText.text = isNewHighscore ? "NEW RECORD!" : "GAME OVER";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        if (highscoreText != null)
        {
            highscoreText.gameObject.SetActive(true);
            highscoreText.text = $"Best: {highscore}";
        }

        if (timeText != null)
        {
            timeText.gameObject.SetActive(true);
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }

        if (newHighscoreText != null)
        {
            newHighscoreText.gameObject.SetActive(isNewHighscore);
        }
    }

    private void OnDestroy()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
        }

        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
        }
    }
}
