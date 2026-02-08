using UnityEngine;
using DG.Tweening;

public class MatchEffects : MonoBehaviour
{
    public static MatchEffects Instance { get; private set; }

    [Header("Particle Settings")]
    [SerializeField] private int particleCount = 12;
    [SerializeField] private float particleSpeed = 5f;
    [SerializeField] private float particleLifetime = 0.5f;
    [SerializeField] private float particleSize = 0.15f;

    [Header("References")]
    [SerializeField] private Transform effectsParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlayMatchEffect(Vector3 position, Color color)
    {
        for (int i = 0; i < particleCount; i++)
        {
            SpawnParticle(position, color, i);
        }
    }

    public void PlayMatchEffect(Shape shape1, Shape shape2)
    {
        Vector3 midPoint = (shape1.transform.position + shape2.transform.position) / 2f;
        Color color = shape1.Color.ToColor();

        PlayMatchEffect(midPoint, color);
    }

    private void SpawnParticle(Vector3 position, Color color, int index)
    {
        GameObject particle = new GameObject($"Particle_{index}");
        
        if (effectsParent != null)
        {
            particle.transform.SetParent(effectsParent);
        }
        
        particle.transform.position = position;
        particle.transform.localScale = Vector3.one * particleSize;

        SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = color;
        sr.sortingOrder = 100;

        float angle = (index / (float)particleCount) * 360f * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

        float randomSpeed = particleSpeed * Random.Range(0.8f, 1.2f);
        Vector3 targetPos = position + direction * randomSpeed * particleLifetime;

        Sequence seq = DOTween.Sequence();
        seq.Append(particle.transform.DOMove(targetPos, particleLifetime).SetEase(Ease.OutQuad));
        seq.Join(particle.transform.DOScale(0f, particleLifetime).SetEase(Ease.InQuad));
        seq.Join(sr.DOFade(0f, particleLifetime).SetEase(Ease.InQuad));
        seq.OnComplete(() => Destroy(particle));
    }

    private Sprite circleSprite;
    private Sprite CreateCircleSprite()
    {
        if (circleSprite != null) return circleSprite;

        int size = 32;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        
        float center = size / 2f;
        float radius = size / 2f - 1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (dist <= radius)
                {
                    float alpha = Mathf.Clamp01((radius - dist) / 2f);
                    texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        circleSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32);
        return circleSprite;
    }
}
