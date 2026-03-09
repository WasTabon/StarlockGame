using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private GameObject noInternetPanel;
    [SerializeField] private Button retryButton;

    private ContentLoader contentLoader;

    private void Start()
    {
        if (noInternetPanel != null)
            noInternetPanel.SetActive(false);

        if (progressSlider != null)
            progressSlider.value = 0f;

        SetStatus("Загрузка...");
        SetPercent(0f);

        GameObject loaderObj = new GameObject("ContentLoader");
        contentLoader = loaderObj.AddComponent<ContentLoader>();

        contentLoader.OnDownloadProgress += OnProgress;
        contentLoader.OnDownloadComplete += OnComplete;
        contentLoader.OnDownloadError += OnError;
        contentLoader.OnNoInternet += OnNoInternet;

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);

        contentLoader.StartDownload();
    }

    private void OnProgress(float progress)
    {
        if (progressSlider != null)
            progressSlider.value = progress;

        SetPercent(progress);
        SetStatus("Загрузка контента...");

        if (noInternetPanel != null)
            noInternetPanel.SetActive(false);
    }

    private void OnComplete()
    {
        SetStatus("Готово!");
        SetPercent(1f);

        if (progressSlider != null)
            progressSlider.value = 1f;

        Invoke("LoadMainMenu", 0.5f);
    }

    private void OnError(string error)
    {
        SetStatus("Ошибка: " + error);
        Debug.LogError("ContentLoader error: " + error);

        if (noInternetPanel != null)
            noInternetPanel.SetActive(true);
    }

    private void OnNoInternet()
    {
        SetStatus("Нет подключения к интернету");

        if (noInternetPanel != null)
            noInternetPanel.SetActive(true);
    }

    private void OnRetryClicked()
    {
        if (noInternetPanel != null)
            noInternetPanel.SetActive(false);

        SetStatus("Повторная попытка...");
        SetPercent(0f);

        if (progressSlider != null)
            progressSlider.value = 0f;

        contentLoader.Retry();
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void SetStatus(string text)
    {
        if (statusText != null)
            statusText.text = text;
    }

    private void SetPercent(float progress)
    {
        if (percentText != null)
            percentText.text = $"{(int)(progress * 100)}%";
    }

    private void OnDestroy()
    {
        if (retryButton != null)
            retryButton.onClick.RemoveAllListeners();

        if (contentLoader != null)
        {
            contentLoader.OnDownloadProgress -= OnProgress;
            contentLoader.OnDownloadComplete -= OnComplete;
            contentLoader.OnDownloadError -= OnError;
            contentLoader.OnNoInternet -= OnNoInternet;
        }
    }
}
