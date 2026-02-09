using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    private const string LEVEL_COMPLETED_KEY = "LevelCompleted_";
    private const string LEVEL_STARS_KEY = "LevelStars_";
    private const string LEVEL_HIGHSCORE_KEY = "LevelHighscore_";
    private const string MAX_UNLOCKED_LEVEL_KEY = "MaxUnlockedLevel";

    public int MaxUnlockedLevel { get; private set; } = 1;

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

        LoadProgress();
    }

    private void LoadProgress()
    {
        MaxUnlockedLevel = PlayerPrefs.GetInt(MAX_UNLOCKED_LEVEL_KEY, 1);
    }

    public bool IsLevelUnlocked(int level)
    {
        return level <= MaxUnlockedLevel;
    }

    public bool IsLevelCompleted(int level)
    {
        return PlayerPrefs.GetInt(LEVEL_COMPLETED_KEY + level, 0) == 1;
    }

    public int GetLevelStars(int level)
    {
        return PlayerPrefs.GetInt(LEVEL_STARS_KEY + level, 0);
    }

    public int GetLevelHighscore(int level)
    {
        return PlayerPrefs.GetInt(LEVEL_HIGHSCORE_KEY + level, 0);
    }

    public void CompleteLevel(int level, int score, int stars)
    {
        PlayerPrefs.SetInt(LEVEL_COMPLETED_KEY + level, 1);

        int previousStars = GetLevelStars(level);
        if (stars > previousStars)
        {
            PlayerPrefs.SetInt(LEVEL_STARS_KEY + level, stars);
        }

        int previousHighscore = GetLevelHighscore(level);
        if (score > previousHighscore)
        {
            PlayerPrefs.SetInt(LEVEL_HIGHSCORE_KEY + level, score);
        }

        int nextLevel = level + 1;
        if (nextLevel > MaxUnlockedLevel)
        {
            MaxUnlockedLevel = nextLevel;
            PlayerPrefs.SetInt(MAX_UNLOCKED_LEVEL_KEY, MaxUnlockedLevel);
        }

        PlayerPrefs.Save();

        Debug.Log($"Level {level} completed! Stars: {stars}, Score: {score}, Next unlocked: {MaxUnlockedLevel}");
    }

    public int CalculateStars(int level, int score, float timeRemaining)
    {
        LevelConfig config = LevelConfig.GetConfig(level);
        if (config == null) return 1;

        int baseScore = config.pairsToSpawn * 100;

        if (score >= baseScore * 1.5f)
        {
            return 3;
        }
        else if (score >= baseScore)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    public void ResetAllProgress()
    {
        for (int i = 1; i <= 20; i++)
        {
            PlayerPrefs.DeleteKey(LEVEL_COMPLETED_KEY + i);
            PlayerPrefs.DeleteKey(LEVEL_STARS_KEY + i);
            PlayerPrefs.DeleteKey(LEVEL_HIGHSCORE_KEY + i);
        }

        PlayerPrefs.SetInt(MAX_UNLOCKED_LEVEL_KEY, 1);
        MaxUnlockedLevel = 1;

        PlayerPrefs.Save();

        Debug.Log("All progress reset!");
    }

    public void UnlockAllLevels()
    {
        MaxUnlockedLevel = 10;
        PlayerPrefs.SetInt(MAX_UNLOCKED_LEVEL_KEY, MaxUnlockedLevel);
        PlayerPrefs.Save();

        Debug.Log("All levels unlocked!");
    }
}
