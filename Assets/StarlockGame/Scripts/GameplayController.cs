using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RotationController rotationController;
    [SerializeField] private CircleContainer circleContainer;
    [SerializeField] private OuterZone outerZone;
    [SerializeField] private GameplayUI gameplayUI;

    [Header("Rotation Presets")]
    [SerializeField] private RotationPreset slowPreset;
    [SerializeField] private RotationPreset mediumPreset;
    [SerializeField] private RotationPreset fastPreset;
    [SerializeField] private RotationPreset insanePreset;

    [Header("Settings")]
    [SerializeField] private int maxShapesInside = 10;

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

    private void OnContainerFull()
    {
        Debug.Log("Container is full! Game Over!");
    }

    private void OnAllShapesCleared()
    {
        Debug.Log("All shapes cleared! Victory!");
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

    public CircleContainer GetCircleContainer() => circleContainer;
    public OuterZone GetOuterZone() => outerZone;
    public RotationController GetRotationController() => rotationController;
}
