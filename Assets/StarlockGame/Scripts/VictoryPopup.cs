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

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(() => OnNextLevelClicked?.Invoke());
        }

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

    public void Setup(int score, bool hasNextLevel)
    {
        SetupButtons();

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
