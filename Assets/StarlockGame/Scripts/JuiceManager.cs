using UnityEngine;

public class JuiceManager : MonoBehaviour
{
    public static JuiceManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private MatchEffects matchEffects;
    [SerializeField] private ScreenShake screenShake;
    [SerializeField] private ScorePopup scorePopup;
    [SerializeField] private GameAudio gameAudio;
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameplayController gameplayController;

    [Header("Settings")]
    [SerializeField] private bool enableEffects = true;
    [SerializeField] private bool enableSounds = true;
    [SerializeField] private bool enableScreenShake = true;

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
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (matchManager != null)
        {
            matchManager.OnMatchFound -= OnMatchFound;
            matchManager.OnMatchFound += OnMatchFound;
        }

        if (inputManager != null)
        {
            inputManager.OnShapeTapped -= OnShapeTapped;
            inputManager.OnShapeTapped += OnShapeTapped;
        }
    }

    private void OnDestroy()
    {
        if (matchManager != null)
        {
            matchManager.OnMatchFound -= OnMatchFound;
        }

        if (inputManager != null)
        {
            inputManager.OnShapeTapped -= OnShapeTapped;
        }
    }

    private void OnShapeTapped(Shape shape)
    {
        if (enableSounds && gameAudio != null)
        {
            gameAudio.PlayTap();
        }
    }

    private void OnMatchFound(Shape shape1, Shape shape2, int points)
    {
        Vector3 midPoint = (shape1.transform.position + shape2.transform.position) / 2f;
        Color color = shape1.Color.ToColor();

        if (enableEffects && matchEffects != null)
        {
            matchEffects.PlayMatchEffect(midPoint, color);
        }

        if (enableEffects && scorePopup != null)
        {
            scorePopup.ShowPopup(midPoint, points, color);
        }

        if (enableSounds && gameAudio != null)
        {
            gameAudio.PlayMatch();
        }

        if (enableScreenShake && screenShake != null)
        {
            screenShake.ShakeLight();
        }
    }

    public void PlayVictoryEffects()
    {
        if (enableSounds && gameAudio != null)
        {
            gameAudio.PlayVictory();
        }
    }

    public void PlayGameOverEffects()
    {
        if (enableSounds && gameAudio != null)
        {
            gameAudio.PlayGameOver();
        }

        if (enableScreenShake && screenShake != null)
        {
            screenShake.ShakeHeavy();
        }
    }

    public void PlayShapeEnterEffect()
    {
        if (enableSounds && gameAudio != null)
        {
            gameAudio.PlayShapeEnter();
        }
    }

    public void SetEffectsEnabled(bool enabled)
    {
        enableEffects = enabled;
    }

    public void SetSoundsEnabled(bool enabled)
    {
        enableSounds = enabled;
    }

    public void SetScreenShakeEnabled(bool enabled)
    {
        enableScreenShake = enabled;
    }
}
