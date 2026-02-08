using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RotationController rotationController;
    [SerializeField] private CircleContainer circleContainer;
    [SerializeField] private OuterZone outerZone;
    [SerializeField] private GameplayUI gameplayUI;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private ShapeSpawner shapeSpawner;
    [SerializeField] private JuiceManager juiceManager;

    [Header("Rotation Presets")]
    [SerializeField] private RotationPreset slowPreset;
    [SerializeField] private RotationPreset mediumPreset;
    [SerializeField] private RotationPreset fastPreset;
    [SerializeField] private RotationPreset insanePreset;

    [Header("Settings")]
    [SerializeField] private int maxShapesInside = 10;
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private float endGameDelay = 0.5f;

    private bool isGameOver = false;
    private bool isVictory = false;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        isGameOver = false;
        isVictory = false;

        SetupRotationForCurrentLevel();
        SetupCircleContainer();
        SubscribeToEvents();

        if (inputManager != null)
        {
            inputManager.SetInputEnabled(true);
        }

        if (gameplayUI != null)
        {
            gameplayUI.HideAllPopups();
        }
    }

    private void SetupRotationForCurrentLevel()
    {
        if (rotationController == null)
        {
            Debug.LogWarning("GameplayController: RotationController not assigned!");
            return;
        }

        RotationPreset preset = GetPresetForLevel(GetCurrentLevel());
        rotationController.SetPreset(preset);
        rotationController.Resume();
    }

    private void SetupCircleContainer()
    {
        if (circleContainer == null)
        {
            Debug.LogWarning("GameplayController: CircleContainer not assigned!");
            return;
        }

        circleContainer.SetMaxShapes(maxShapesInside);
    }

    private void SubscribeToEvents()
    {
        if (circleContainer != null)
        {
            circleContainer.OnContainerFull -= OnContainerFull;
            circleContainer.OnContainerFull += OnContainerFull;
        }

        if (outerZone != null)
        {
            outerZone.OnAllShapesCleared -= OnAllShapesCleared;
            outerZone.OnAllShapesCleared += OnAllShapesCleared;
        }

        if (inputManager != null)
        {
            inputManager.OnShapeTapped -= OnShapeTapped;
            inputManager.OnShapeTapped += OnShapeTapped;
        }

        if (matchManager != null)
        {
            matchManager.OnScoreChanged -= OnScoreChanged;
            matchManager.OnScoreChanged += OnScoreChanged;

            matchManager.OnMatchFound -= OnMatchFound;
            matchManager.OnMatchFound += OnMatchFound;
        }

        if (gameplayUI != null)
        {
            gameplayUI.OnRestartClicked -= OnRestartClicked;
            gameplayUI.OnRestartClicked += OnRestartClicked;

            gameplayUI.OnNextLevelClicked -= OnNextLevelClicked;
            gameplayUI.OnNextLevelClicked += OnNextLevelClicked;

            gameplayUI.OnMenuClicked -= OnMenuClicked;
            gameplayUI.OnMenuClicked += OnMenuClicked;
        }
    }

    private void OnDestroy()
    {
        if (circleContainer != null)
        {
            circleContainer.OnContainerFull -= OnContainerFull;
        }

        if (outerZone != null)
        {
            outerZone.OnAllShapesCleared -= OnAllShapesCleared;
        }

        if (inputManager != null)
        {
            inputManager.OnShapeTapped -= OnShapeTapped;
        }

        if (matchManager != null)
        {
            matchManager.OnScoreChanged -= OnScoreChanged;
            matchManager.OnMatchFound -= OnMatchFound;
        }

        if (gameplayUI != null)
        {
            gameplayUI.OnRestartClicked -= OnRestartClicked;
            gameplayUI.OnNextLevelClicked -= OnNextLevelClicked;
            gameplayUI.OnMenuClicked -= OnMenuClicked;
        }
    }

    private int GetCurrentLevel()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.CurrentMode == GameMode.Levels)
            {
                return GameManager.Instance.SelectedLevel;
            }
        }
        return 1;
    }

    private RotationPreset GetPresetForLevel(int level)
    {
        if (level <= 3)
        {
            return slowPreset;
        }
        else if (level <= 6)
        {
            return mediumPreset;
        }
        else if (level <= 9)
        {
            return fastPreset;
        }
        else
        {
            return insanePreset;
        }
    }

    private void OnShapeTapped(Shape shape)
    {
        if (isGameOver || isVictory) return;

        shape.OnEnteredCircle -= OnShapeEnteredCircle;
        shape.OnEnteredCircle += OnShapeEnteredCircle;
    }

    private void OnShapeEnteredCircle(Shape shape)
    {
        shape.OnEnteredCircle -= OnShapeEnteredCircle;

        if (matchManager != null)
        {
            matchManager.RegisterShapeInside(shape);
        }

        if (juiceManager != null)
        {
            juiceManager.PlayShapeEnterEffect();
        }
    }

    private void OnScoreChanged(int newScore)
    {
        if (gameplayUI != null)
        {
            gameplayUI.UpdateScore(newScore);
        }
    }

    private void OnMatchFound(Shape shape1, Shape shape2, int points)
    {
        Debug.Log($"Match! +{points} points");

        if (circleContainer != null)
        {
            circleContainer.RemoveShapeInside(shape1.gameObject);
            circleContainer.RemoveShapeInside(shape2.gameObject);
        }

        if (outerZone != null)
        {
            outerZone.RemoveShapeOutside(shape1.gameObject);
            outerZone.RemoveShapeOutside(shape2.gameObject);
        }

        Invoke(nameof(CheckVictoryCondition), endGameDelay);
    }

    private void OnContainerFull()
    {
        if (isGameOver || isVictory) return;

        isGameOver = true;
        Debug.Log("GAME OVER - Container is full!");

        if (rotationController != null)
        {
            rotationController.Stop();
        }

        if (inputManager != null)
        {
            inputManager.SetInputEnabled(false);
        }

        if (juiceManager != null)
        {
            juiceManager.PlayGameOverEffects();
        }

        Invoke(nameof(ShowGameOverPopup), endGameDelay);
    }

    private void OnAllShapesCleared()
    {
        if (isGameOver || isVictory) return;

        CheckVictoryCondition();
    }

    private void CheckVictoryCondition()
    {
        if (isGameOver || isVictory) return;

        if (shapeSpawner == null) return;

        int remainingShapes = shapeSpawner.GetSpawnedCount();
        int shapesInside = matchManager != null ? matchManager.GetShapesInsideCount() : 0;

        if (remainingShapes == 0 && shapesInside == 0)
        {
            isVictory = true;
            Debug.Log("VICTORY - All shapes cleared!");

            if (rotationController != null)
            {
                rotationController.Stop();
            }

            if (inputManager != null)
            {
                inputManager.SetInputEnabled(false);
            }

            if (juiceManager != null)
            {
                juiceManager.PlayVictoryEffects();
            }

            Invoke(nameof(ShowVictoryPopup), endGameDelay);
        }
    }

    private void ShowVictoryPopup()
    {
        if (gameplayUI == null) return;

        int score = matchManager != null ? matchManager.CurrentScore : 0;
        bool hasNextLevel = GetCurrentLevel() < maxLevel && 
                           (GameManager.Instance == null || GameManager.Instance.CurrentMode == GameMode.Levels);

        gameplayUI.ShowVictoryPopup(score, hasNextLevel);
    }

    private void ShowGameOverPopup()
    {
        if (gameplayUI == null) return;

        int score = matchManager != null ? matchManager.CurrentScore : 0;
        gameplayUI.ShowGameOverPopup(score);
    }

    private void OnRestartClicked()
    {
        Debug.Log("Restart clicked");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnNextLevelClicked()
    {
        Debug.Log("Next Level clicked");

        if (GameManager.Instance != null)
        {
            int nextLevel = GameManager.Instance.SelectedLevel + 1;
            if (nextLevel <= maxLevel)
            {
                GameManager.Instance.StartLevelMode(nextLevel);
                return;
            }
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMenuClicked()
    {
        Debug.Log("Menu clicked");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void SetRotationPreset(RotationPreset preset)
    {
        if (rotationController != null)
        {
            rotationController.SetPreset(preset);
        }
    }

    public void PauseRotation()
    {
        if (rotationController != null)
        {
            rotationController.Stop();
        }
    }

    public void ResumeRotation()
    {
        if (rotationController != null)
        {
            rotationController.Resume();
        }
    }

    public bool IsGameOver => isGameOver;
    public bool IsVictory => isVictory;

    public CircleContainer GetCircleContainer() => circleContainer;
    public OuterZone GetOuterZone() => outerZone;
    public RotationController GetRotationController() => rotationController;
}
