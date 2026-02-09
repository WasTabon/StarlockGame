using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePopup : UIPanel
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Toggle soundToggle;

    public System.Action OnResumeClicked;
    public System.Action OnRestartClicked;
    public System.Action OnMenuClicked;

    protected override void Awake()
    {
        base.Awake();
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(OnResumePressed);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartPressed);
        }

        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OnMenuPressed);
        }

        if (soundToggle != null)
        {
            soundToggle.onValueChanged.RemoveAllListeners();
            soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
        }
    }

    private void OnResumePressed()
    {
        Debug.Log("Resume pressed");
        OnResumeClicked?.Invoke();
    }

    private void OnRestartPressed()
    {
        Debug.Log("Restart pressed");
        OnRestartClicked?.Invoke();
    }

    private void OnMenuPressed()
    {
        Debug.Log("Menu pressed");
        OnMenuClicked?.Invoke();
    }

    private void OnEnable()
    {
        UpdateSoundToggle();
    }

    private void UpdateSoundToggle()
    {
        if (soundToggle == null) return;

        if (GameManager.Instance != null)
        {
            soundToggle.isOn = GameManager.Instance.SoundEnabled;
        }
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
        if (resumeButton != null)
            resumeButton.onClick.RemoveAllListeners();

        if (restartButton != null)
            restartButton.onClick.RemoveAllListeners();

        if (menuButton != null)
            menuButton.onClick.RemoveAllListeners();

        if (soundToggle != null)
            soundToggle.onValueChanged.RemoveAllListeners();
    }
}
