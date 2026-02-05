using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPopup : UIPanel
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    public System.Action OnRestartClicked;
    public System.Action OnMenuClicked;

    protected override void Awake()
    {
        base.Awake();
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());
        }
    }

    public void Setup(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
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
