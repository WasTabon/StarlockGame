using UnityEngine;

public class EndlessManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShapeSpawner shapeSpawner;
    [SerializeField] private RotationController rotationController;
    [SerializeField] private MatchManager matchManager;

    [Header("Spawn Settings")]
    [SerializeField] private float initialSpawnInterval = 3f;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float spawnIntervalDecreaseRate = 0.05f;
    [SerializeField] private int maxShapesOnScreen = 20;

    [Header("Difficulty Settings")]
    [SerializeField] private float initialRotationSpeed = 30f;
    [SerializeField] private float maxRotationSpeed = 100f;
    [SerializeField] private float rotationSpeedIncreaseRate = 1f;
    [SerializeField] private float difficultyIncreaseInterval = 10f;

    [Header("Runtime")]
    [SerializeField] private float currentSpawnInterval;
    [SerializeField] private float currentRotationSpeed;
    [SerializeField] private float gameTime;
    [SerializeField] private int difficultyLevel;

    private float spawnTimer;
    private float difficultyTimer;
    private bool isRunning = false;

    public float GameTime => gameTime;
    public int DifficultyLevel => difficultyLevel;

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentMode == GameMode.Endless)
        {
            StartEndlessMode();
        }
    }

    public void StartEndlessMode()
    {
        currentSpawnInterval = initialSpawnInterval;
        currentRotationSpeed = initialRotationSpeed;
        gameTime = 0f;
        difficultyLevel = 1;
        spawnTimer = 0f;
        difficultyTimer = 0f;
        isRunning = true;

        if (rotationController != null)
        {
            rotationController.SetSpeed(currentRotationSpeed);
        }

        SpawnInitialPairs();
    }

    private void SpawnInitialPairs()
    {
        if (shapeSpawner == null) return;

        for (int i = 0; i < 6; i++)
        {
            shapeSpawner.SpawnRandomShape();
        }
    }

    private void Update()
    {
        if (!isRunning) return;

        gameTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;
        difficultyTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnInterval)
        {
            TrySpawnPair();
            spawnTimer = 0f;
        }

        if (difficultyTimer >= difficultyIncreaseInterval)
        {
            IncreaseDifficulty();
            difficultyTimer = 0f;
        }
    }

    private void TrySpawnPair()
    {
        if (shapeSpawner == null) return;

        int currentShapes = shapeSpawner.GetSpawnedCount();
        if (currentShapes >= maxShapesOnScreen)
        {
            return;
        }

        shapeSpawner.SpawnRandomShape();
    }

    private void IncreaseDifficulty()
    {
        difficultyLevel++;

        currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - spawnIntervalDecreaseRate);

        currentRotationSpeed = Mathf.Min(maxRotationSpeed, currentRotationSpeed + rotationSpeedIncreaseRate);

        if (rotationController != null)
        {
            bool reverseDirection = (difficultyLevel % 5 == 0);
            if (reverseDirection)
            {
                rotationController.SetDirection(difficultyLevel % 10 < 5);
            }

            rotationController.SetSpeed(currentRotationSpeed);
        }

        Debug.Log($"Difficulty increased! Level: {difficultyLevel}, Spawn: {currentSpawnInterval:F2}s, Speed: {currentRotationSpeed:F1}");
    }

    public void StopEndlessMode()
    {
        isRunning = false;
    }

    public void PauseEndlessMode()
    {
        isRunning = false;
    }

    public void ResumeEndlessMode()
    {
        isRunning = true;
    }

    public int GetFinalScore()
    {
        if (matchManager != null)
        {
            return matchManager.CurrentScore;
        }
        return 0;
    }
}
