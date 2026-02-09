using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button nextButton;

    [Header("Settings")]
    [SerializeField] private bool showTutorialOnFirstPlay = true;
    [SerializeField] private float textAnimationSpeed = 0.02f;

    private const string TUTORIAL_COMPLETED_KEY = "TutorialCompleted";

    private int currentStep = 0;
    private bool isTutorialActive = false;
    private System.Action onTutorialComplete;
    private Coroutine typeCoroutine;

    private string[] tutorialSteps = new string[]
    {
        "Welcome to Starlock!\n\nTap shapes in the outer ring to send them inside the circle.",
        "Match two identical shapes to clear them!\n\nSame shape + same color = match!",
        "Don't let the circle fill up!\n\nIf too many shapes are inside, you lose.",
        "Clear all shapes to win!\n\nGood luck and have fun!"
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetupButtons();
        
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    private void SetupButtons()
    {
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(SkipTutorial);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(NextStep);
        }
    }

    public bool ShouldShowTutorial()
    {
        if (!showTutorialOnFirstPlay) return false;
        return PlayerPrefs.GetInt(TUTORIAL_COMPLETED_KEY, 0) == 0;
    }

    public void StartTutorial(System.Action onComplete = null)
    {
        onTutorialComplete = onComplete;
        currentStep = 0;
        isTutorialActive = true;

        SetupButtons();
        ShowTutorialPanel();
        ShowStep(currentStep);
    }

    private void ShowTutorialPanel()
    {
        if (tutorialPanel == null) return;

        tutorialPanel.SetActive(true);

        CanvasGroup cg = tutorialPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            cg.DOFade(1f, 0.3f);
        }
    }

    public void HideTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
            typeCoroutine = null;
        }
    }

    private void ShowStep(int step)
    {
        if (step < 0 || step >= tutorialSteps.Length) return;

        if (tutorialText != null)
        {
            if (typeCoroutine != null)
            {
                StopCoroutine(typeCoroutine);
            }
            typeCoroutine = StartCoroutine(TypeText(tutorialSteps[step]));
        }

        if (nextButton != null)
        {
            TextMeshProUGUI btnText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = (step == tutorialSteps.Length - 1) ? "START" : "NEXT";
            }
        }
    }

    private System.Collections.IEnumerator TypeText(string text)
    {
        tutorialText.text = "";
        foreach (char c in text)
        {
            tutorialText.text += c;
            yield return new WaitForSecondsRealtime(textAnimationSpeed);
        }
    }

    private void NextStep()
    {
        currentStep++;

        if (currentStep >= tutorialSteps.Length)
        {
            CompleteTutorial();
        }
        else
        {
            ShowStep(currentStep);
        }
    }

    private void SkipTutorial()
    {
        CompleteTutorial();
    }

    private void CompleteTutorial()
    {
        isTutorialActive = false;

        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
            typeCoroutine = null;
        }

        PlayerPrefs.SetInt(TUTORIAL_COMPLETED_KEY, 1);
        PlayerPrefs.Save();

        if (tutorialPanel != null)
        {
            CanvasGroup cg = tutorialPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOFade(0f, 0.3f).OnComplete(() =>
                {
                    tutorialPanel.SetActive(false);
                    onTutorialComplete?.Invoke();
                });
            }
            else
            {
                tutorialPanel.SetActive(false);
                onTutorialComplete?.Invoke();
            }
        }
        else
        {
            onTutorialComplete?.Invoke();
        }
    }

    public void ResetTutorial()
    {
        PlayerPrefs.SetInt(TUTORIAL_COMPLETED_KEY, 0);
        PlayerPrefs.Save();
        Debug.Log("Tutorial reset!");
    }

    public bool IsTutorialActive => isTutorialActive;

    private void OnDestroy()
    {
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
        }
    }
}
