using UnityEngine;
using DG.Tweening;

    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float showDuration = 0.3f;
        [SerializeField] private float hideDuration = 0.2f;
        [SerializeField] private Ease showEase = Ease.OutBack;
        [SerializeField] private Ease hideEase = Ease.InQuad;

        [Header("Animation Type")]
        [SerializeField] private bool useScaleAnimation = true;
        [SerializeField] private bool useFadeAnimation = true;
        [SerializeField] private float startScale = 0.8f;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Tween showTween;
        private Tween hideTween;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
        }

        protected virtual void OnEnable()
        {
            Show();
        }

        public virtual void Show(System.Action onComplete = null)
        {
            gameObject.SetActive(true);
            
            showTween?.Kill();
            hideTween?.Kill();

            if (useScaleAnimation)
            {
                rectTransform.localScale = Vector3.one * startScale;
                showTween = rectTransform.DOScale(Vector3.one, showDuration)
                    .SetEase(showEase)
                    .SetUpdate(true);
            }

            if (useFadeAnimation)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1f, showDuration)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)
                    .OnComplete(() => onComplete?.Invoke());
            }
            else
            {
                showTween?.OnComplete(() => onComplete?.Invoke());
            }

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPanelOpen();
            }
        }

        public virtual void Hide(System.Action onComplete = null)
        {
            showTween?.Kill();
            hideTween?.Kill();

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            if (useScaleAnimation)
            {
                hideTween = rectTransform.DOScale(Vector3.one * startScale, hideDuration)
                    .SetEase(hideEase)
                    .SetUpdate(true);
            }

            if (useFadeAnimation)
            {
                canvasGroup.DOFade(0f, hideDuration)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        onComplete?.Invoke();
                    });
            }
            else
            {
                hideTween?.OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPanelClose();
            }
        }

        public void HideInstant()
        {
            showTween?.Kill();
            hideTween?.Kill();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            rectTransform.localScale = Vector3.one * startScale;
            gameObject.SetActive(false);
        }

        public void ShowInstant()
        {
            showTween?.Kill();
            hideTween?.Kill();

            gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            rectTransform.localScale = Vector3.one;
        }
    }
