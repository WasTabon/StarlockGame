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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLevelMode(int levelIndex)
    {
        CurrentMode = GameMode.Levels;
        SelectedLevel = levelIndex;
        LoadGameplayScene();
    }

    public void StartEndlessMode()
    {
        CurrentMode = GameMode.Endless;
        SelectedLevel = 0;
        LoadGameplayScene();
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
