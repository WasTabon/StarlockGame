using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

    public class SceneTransition : MonoBehaviour
    {
        public static SceneTransition Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("References")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private Image fadeImage;

        private bool isTransitioning = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }

        private void Start()
        {
            FadeIn();
        }

        public void LoadScene(string sceneName)
        {
            if (isTransitioning) return;
            StartCoroutine(TransitionToScene(sceneName));
        }

        private System.Collections.IEnumerator TransitionToScene(string sceneName)
        {
            isTransitioning = true;

            yield return FadeOut();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            yield return FadeIn();

            isTransitioning = false;
        }

        private System.Collections.IEnumerator FadeOut()
        {
            if (fadeCanvasGroup == null) yield break;

            fadeCanvasGroup.blocksRaycasts = true;
            yield return fadeCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
        }

        private System.Collections.IEnumerator FadeIn()
        {
            if (fadeCanvasGroup == null) yield break;

            yield return fadeCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
            fadeCanvasGroup.blocksRaycasts = false;
        }

        public void SetFadeColor(Color color)
        {
            if (fadeImage != null)
            {
                fadeImage.color = color;
            }
        }
    }
