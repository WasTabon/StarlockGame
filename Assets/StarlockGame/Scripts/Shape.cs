using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Shape : MonoBehaviour
{
    [Header("Shape Data")]
    [SerializeField] private ShapeType shapeType;
    [SerializeField] private ShapeColor shapeColor;
    [SerializeField] private ShapeState state = ShapeState.Outside;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;

    [Header("Settings")]
    [SerializeField] private float moveInsideDuration = 0.3f;
    [SerializeField] private float matchedScaleDuration = 0.2f;

    public ShapeType Type => shapeType;
    public ShapeColor Color => shapeColor;
    public ShapeState State => state;

    public System.Action<Shape> OnStateChanged;
    public System.Action<Shape> OnMatched;
    public System.Action<Shape> OnEnteredCircle;

    private Tween moveTween;
    private Tween scaleTween;

    private void Awake()
    {
        EnsureReferences();
    }

    private void EnsureReferences()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (col == null)
            col = GetComponent<Collider2D>();
    }

    public void Initialize(ShapeType type, ShapeColor color, Sprite sprite)
    {
        EnsureReferences();

        shapeType = type;
        shapeColor = color;
        state = ShapeState.Outside;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color.ToColor();
        }

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
        }
    }

    public void SetState(ShapeState newState)
    {
        if (state == newState) return;

        state = newState;
        OnStateChanged?.Invoke(this);

        switch (state)
        {
            case ShapeState.Outside:
                EnablePhysics(true);
                break;
            case ShapeState.MovingInside:
                EnablePhysics(false);
                break;
            case ShapeState.Inside:
                EnablePhysics(true);
                OnEnteredCircle?.Invoke(this);
                break;
            case ShapeState.Matched:
                EnablePhysics(false);
                PlayMatchedAnimation();
                break;
        }
    }

    public void MoveInside(Vector2 targetPosition, System.Action onComplete = null)
    {
        if (state != ShapeState.Outside) return;

        SetState(ShapeState.MovingInside);

        moveTween?.Kill();
        moveTween = transform.DOLocalMove(targetPosition, moveInsideDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                SetState(ShapeState.Inside);
                onComplete?.Invoke();
            });
    }

    public void PlayMatchedAnimation(System.Action onComplete = null)
    {
        scaleTween?.Kill();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(Vector3.one * 1.3f, matchedScaleDuration * 0.5f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(Vector3.zero, matchedScaleDuration * 0.5f).SetEase(Ease.InQuad));
        seq.OnComplete(() =>
        {
            OnMatched?.Invoke(this);
            onComplete?.Invoke();
            Destroy(gameObject);
        });

        scaleTween = seq;
    }

    public void ApplyForce(Vector2 force)
    {
        if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (rb != null)
        {
            rb.velocity = velocity;
        }
    }

    private void EnablePhysics(bool enabled)
    {
        if (rb != null)
        {
            rb.bodyType = enabled ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
            if (!enabled)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        if (col != null)
        {
            col.enabled = enabled;
        }
    }

    public bool Matches(Shape other)
    {
        if (other == null) return false;
        return shapeType == other.shapeType && shapeColor == other.shapeColor;
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
        scaleTween?.Kill();
    }

    public void SetSortingOrder(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}
