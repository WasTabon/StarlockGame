using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScorePopup : MonoBehaviour
{
    public static ScorePopup Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float floatDistance = 1f;
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float fontSize = 48f;

    [Header("References")]
    [SerializeField] private Canvas worldCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        EnsureWorldCanvas();
    }

    private void EnsureWorldCanvas()
    {
        if (worldCanvas != null) return;

        GameObject canvasObj = new GameObject("WorldCanvas");
        canvasObj.transform.SetParent(transform);
        
        worldCanvas = canvasObj.AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.WorldSpace;
        worldCanvas.sortingOrder = 200;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.localPosition = Vector3.zero;
        canvasRect.localScale = Vector3.one * 0.01f;
    }

    public void ShowPopup(Vector3 worldPosition, int points, Color color)
    {
        EnsureWorldCanvas();

        GameObject popupObj = new GameObject("ScorePopup");
        popupObj.transform.SetParent(worldCanvas.transform);
        popupObj.transform.position = worldPosition;
        popupObj.transform.localScale = Vector3.one * 100f;

        TextMeshProUGUI text = popupObj.AddComponent<TextMeshProUGUI>();
        text.text = $"+{points}";
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;

        RectTransform rect = popupObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 100);

        Vector3 startPos = popupObj.transform.position;
        Vector3 endPos = startPos + Vector3.up * floatDistance;

        Sequence seq = DOTween.Sequence();
        seq.Append(popupObj.transform.DOMove(endPos, duration).SetEase(Ease.OutQuad));
        seq.Join(popupObj.transform.DOScale(Vector3.one * 150f, duration * 0.3f).SetEase(Ease.OutBack));
        seq.Insert(duration * 0.5f, text.DOFade(0f, duration * 0.5f).SetEase(Ease.InQuad));
        seq.OnComplete(() => Destroy(popupObj));
    }

    public void ShowPopup(Vector3 worldPosition, int points)
    {
        ShowPopup(worldPosition, points, Color.white);
    }
}
