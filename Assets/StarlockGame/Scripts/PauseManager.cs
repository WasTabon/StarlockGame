using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private RotationController rotationController;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private EndlessManager endlessManager;

    private bool isPaused = false;

    public bool IsPaused => isPaused;

    public System.Action OnPaused;
    public System.Action OnResumed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (inputManager != null)
        {
            inputManager.SetInputEnabled(false);
        }

        OnPaused?.Invoke();

        Debug.Log("Game Paused");
    }

    public void Resume()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (inputManager != null)
        {
            inputManager.SetInputEnabled(true);
        }

        OnResumed?.Invoke();

        Debug.Log("Game Resumed");
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && !isPaused)
        {
            Pause();
        }
    }
}
