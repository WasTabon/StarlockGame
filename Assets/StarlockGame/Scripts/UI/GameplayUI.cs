using UnityEngine;
using UnityEngine.UI;
using TMPro;

    public class GameplayUI : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button pauseButton;

        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI modeText;
        [SerializeField] private Button backToMenuButton;

        private void Start()
        {
            UpdateModeDisplay();
            
            if (backToMenuButton != null)
            {
                backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
            }

            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseClicked);
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
            GameManager.Instance.ReturnToMainMenu();
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