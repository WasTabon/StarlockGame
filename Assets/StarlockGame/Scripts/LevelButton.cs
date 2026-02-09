using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI levelNumberText;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private GameObject completedCheckmark;
    [SerializeField] private Transform starsContainer;
    [SerializeField] private Image[] starImages;

    [Header("Colors")]
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    [SerializeField] private Color completedColor = new Color(0.7f, 1f, 0.7f);
    [SerializeField] private Color starActiveColor = Color.yellow;
    [SerializeField] private Color starInactiveColor = new Color(0.3f, 0.3f, 0.3f);

    private int levelNumber;
    private bool isUnlocked;
    private bool isCompleted;
    private int stars;

    public System.Action<int> OnLevelSelected;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    public void Setup(int level)
    {
        levelNumber = level;

        if (levelNumberText != null)
        {
            levelNumberText.text = level.ToString();
        }

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (ProgressManager.Instance == null)
        {
            isUnlocked = levelNumber == 1;
            isCompleted = false;
            stars = 0;
        }
        else
        {
            isUnlocked = ProgressManager.Instance.IsLevelUnlocked(levelNumber);
            isCompleted = ProgressManager.Instance.IsLevelCompleted(levelNumber);
            stars = ProgressManager.Instance.GetLevelStars(levelNumber);
        }

        if (button != null)
        {
            button.interactable = isUnlocked;
        }

        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(!isUnlocked);
        }

        if (completedCheckmark != null)
        {
            completedCheckmark.SetActive(isCompleted);
        }

        Image buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            if (!isUnlocked)
            {
                buttonImage.color = lockedColor;
            }
            else if (isCompleted)
            {
                buttonImage.color = completedColor;
            }
            else
            {
                buttonImage.color = unlockedColor;
            }
        }

        UpdateStars();
    }

    private void UpdateStars()
    {
        if (starImages == null || starImages.Length == 0) return;

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                starImages[i].color = (i < stars) ? starActiveColor : starInactiveColor;
            }
        }

        if (starsContainer != null)
        {
            starsContainer.gameObject.SetActive(isCompleted);
        }
    }

    private void OnButtonClicked()
    {
        if (!isUnlocked) return;

        OnLevelSelected?.Invoke(levelNumber);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
