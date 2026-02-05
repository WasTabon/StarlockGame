using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryPopup : UIPanel
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    public System.Action OnNextLevelClicked;
    public System.Action OnRestartClicked;
    public System.Action OnMenuClicked;

    protected override void Awake()
    {
        base.Awake();
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(() => OnNextLevelClicked?.Invoke());
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());
        }
    }

    public void Setup(int score, bool hasNextLevel)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(hasNextLevel);
        }
    }

    private void OnDestroy()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
        }

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
