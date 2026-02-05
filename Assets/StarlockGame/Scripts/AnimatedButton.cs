using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class AnimatedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Animation")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float pressedScale = 0.95f;
    [SerializeField] private float animationDuration = 0.15f;

    [Header("Sound")]
    [SerializeField] private bool playClickSound = true;
    [SerializeField] private bool playHoverSound = true;

    private Vector3 originalScale;
    private Button button;
    private Tween currentTween;

    private void Awake()
    {
        originalScale = transform.localScale;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * hoverScale, animationDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        if (playHoverSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonHover();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable) return;

        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, animationDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;

        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * pressedScale, animationDuration * 0.5f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;

        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * hoverScale, animationDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    private void OnClick()
    {
        if (playClickSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    public void ResetScale()
    {
        currentTween?.Kill();
        transform.localScale = originalScale;
    }
}
