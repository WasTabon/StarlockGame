using UnityEngine;

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

    [Header("Rotation Presets")]
    [SerializeField] private RotationPreset slowPreset;
    [SerializeField] private RotationPreset mediumPreset;
    [SerializeField] private RotationPreset fastPreset;
    [SerializeField] private RotationPreset insanePreset;

    [Header("Settings")]
    [SerializeField] private int maxShapesInside = 10;

    private bool isGameOver = false;
    private bool isVictory = false;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        SetupRotationForCurrentLevel();
        SetupCircleContainer();
        SubscribeToEvents();
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

        if (outerZone != null)
        {
            outerZone.RemoveShapeOutside(shape1.gameObject);
            outerZone.RemoveShapeOutside(shape2.gameObject);
        }

        CheckVictoryCondition();
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
    }

    private void OnAllShapesCleared()
    {
        if (isGameOver || isVictory) return;

        CheckVictoryCondition();
    }

    private void CheckVictoryCondition()
    {
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
