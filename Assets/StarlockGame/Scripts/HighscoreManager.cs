using UnityEngine;
using System.Collections.Generic;

public class HighscoreManager : MonoBehaviour
{
    public static HighscoreManager Instance { get; private set; }

    private const string ENDLESS_HIGHSCORE_KEY = "EndlessHighscore";
    private const string ENDLESS_HIGHSCORES_KEY = "EndlessHighscores_";
    private const int MAX_HIGHSCORES = 5;

    public int EndlessHighscore { get; private set; }

    private List<HighscoreEntry> highscores = new List<HighscoreEntry>();

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

        LoadHighscores();
    }

    private void LoadHighscores()
    {
        EndlessHighscore = PlayerPrefs.GetInt(ENDLESS_HIGHSCORE_KEY, 0);

        highscores.Clear();
        for (int i = 0; i < MAX_HIGHSCORES; i++)
        {
            int score = PlayerPrefs.GetInt(ENDLESS_HIGHSCORES_KEY + i + "_score", 0);
            float time = PlayerPrefs.GetFloat(ENDLESS_HIGHSCORES_KEY + i + "_time", 0f);
            
            if (score > 0)
            {
                highscores.Add(new HighscoreEntry(score, time));
            }
        }
    }

    public bool SubmitScore(int score, float gameTime)
    {
        bool isNewHighscore = score > EndlessHighscore;

        if (isNewHighscore)
        {
            EndlessHighscore = score;
            PlayerPrefs.SetInt(ENDLESS_HIGHSCORE_KEY, EndlessHighscore);
        }

        highscores.Add(new HighscoreEntry(score, gameTime));
        highscores.Sort((a, b) => b.score.CompareTo(a.score));

        while (highscores.Count > MAX_HIGHSCORES)
        {
            highscores.RemoveAt(highscores.Count - 1);
        }

        SaveHighscores();

        Debug.Log($"Score submitted: {score}. New highscore: {isNewHighscore}");

        return isNewHighscore;
    }

    private void SaveHighscores()
    {
        for (int i = 0; i < MAX_HIGHSCORES; i++)
        {
            if (i < highscores.Count)
            {
                PlayerPrefs.SetInt(ENDLESS_HIGHSCORES_KEY + i + "_score", highscores[i].score);
                PlayerPrefs.SetFloat(ENDLESS_HIGHSCORES_KEY + i + "_time", highscores[i].time);
            }
            else
            {
                PlayerPrefs.DeleteKey(ENDLESS_HIGHSCORES_KEY + i + "_score");
                PlayerPrefs.DeleteKey(ENDLESS_HIGHSCORES_KEY + i + "_time");
            }
        }

        PlayerPrefs.Save();
    }

    public List<HighscoreEntry> GetHighscores()
    {
        return new List<HighscoreEntry>(highscores);
    }

    public int GetRank(int score)
    {
        for (int i = 0; i < highscores.Count; i++)
        {
            if (score >= highscores[i].score)
            {
                return i + 1;
            }
        }
        return highscores.Count + 1;
    }

    public void ResetHighscores()
    {
        EndlessHighscore = 0;
        highscores.Clear();

        PlayerPrefs.SetInt(ENDLESS_HIGHSCORE_KEY, 0);
        for (int i = 0; i < MAX_HIGHSCORES; i++)
        {
            PlayerPrefs.DeleteKey(ENDLESS_HIGHSCORES_KEY + i + "_score");
            PlayerPrefs.DeleteKey(ENDLESS_HIGHSCORES_KEY + i + "_time");
        }
        PlayerPrefs.Save();

        Debug.Log("Highscores reset!");
    }
}

[System.Serializable]
public class HighscoreEntry
{
    public int score;
    public float time;

    public HighscoreEntry(int score, float time)
    {
        this.score = score;
        this.time = time;
    }

    public string GetTimeString()
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
