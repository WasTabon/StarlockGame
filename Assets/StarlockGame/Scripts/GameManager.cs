using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    None,
    Levels,
    Endless
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMode CurrentMode { get; private set; } = GameMode.None;
    public int SelectedLevel { get; private set; } = 1;
    public bool SoundEnabled { get; set; } = true;

    public LevelConfig CurrentLevelConfig { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void StartLevelMode(int levelIndex)
    {
        CurrentMode = GameMode.Levels;
        SelectedLevel = levelIndex;
        CurrentLevelConfig = LevelConfig.GetConfig(levelIndex);
        LoadGameplayScene();
    }

    public void StartEndlessMode()
    {
        CurrentMode = GameMode.Endless;
        SelectedLevel = 0;
        CurrentLevelConfig = new LevelConfig
        {
            levelNumber = 0,
            pairsToSpawn = 10,
            rotationSpeed = 40f,
            reverseRotation = false,
            maxShapesInside = 10
        };
        LoadGameplayScene();
    }

    public void OnLevelCompleted(int score)
    {
        if (CurrentMode != GameMode.Levels) return;
        if (ProgressManager.Instance == null) return;

        int stars = ProgressManager.Instance.CalculateStars(SelectedLevel, score, 0f);
        ProgressManager.Instance.CompleteLevel(SelectedLevel, score, stars);
    }

    public bool HasNextLevel()
    {
        if (CurrentMode != GameMode.Levels) return false;
        return SelectedLevel < LevelConfig.GetTotalLevels();
    }

    public void ReturnToMainMenu()
    {
        CurrentMode = GameMode.None;
        SceneTransition.Instance.LoadScene("MainMenu");
    }

    private void LoadGameplayScene()
    {
        SceneTransition.Instance.LoadScene("Gameplay");
    }
}
