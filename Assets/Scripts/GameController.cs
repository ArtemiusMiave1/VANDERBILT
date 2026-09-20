using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Game Systems")]
    public GameClock gameClock;

    [Header("Play Time")]
    public float playTime = 0f;

    [Header("Game State")]
    public bool gameRunning = true;
    public bool gamePaused = false;

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

    private void Start()
    {
        FindMissingReferences();

        StartGame();
    }

    private void Update()
    {
        if (!gameRunning)
            return;

        UpdatePlayTime();
        HandleGameInput();
    }

    private void UpdatePlayTime()
    {
        if (gamePaused)
            return;

        playTime += Time.unscaledDeltaTime;
    }

    private void HandleGameInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void StartGame()
    {
        gameRunning = true;
        gamePaused = false;

        Time.timeScale = 1f;

        Debug.Log("Vanderbilt Game Started");
    }

    public void PauseGame()
    {
        if (!gameRunning)
            return;

        gamePaused = true;

        Time.timeScale = 0f;

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (!gameRunning)
            return;

        gamePaused = false;

        Time.timeScale = 1f;

        Debug.Log("Game Resumed");
    }

    public void TogglePause()
    {
        if (gamePaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void EndGame()
    {
        gameRunning = false;

        Time.timeScale = 0f;

        Debug.Log(
            "Game Ended. Total Play Time: " +
            GetFormattedPlayTime()
        );
    }

    // Returns play time in seconds.
    public float GetPlayTime()
    {
        return playTime;
    }

    // Returns play time as HH:MM:SS.
    public string GetFormattedPlayTime()
    {
        int hours = Mathf.FloorToInt(playTime / 3600f);

        int minutes =
            Mathf.FloorToInt(
                (playTime % 3600f) / 60f
            );

        int seconds =
            Mathf.FloorToInt(
                playTime % 60f
            );

        return string.Format(
            "{0:00}:{1:00}:{2:00}",
            hours,
            minutes,
            seconds
        );
    }

    private void FindMissingReferences()
    {
        if (gameClock == null)
            gameClock = FindObjectOfType<GameClock>();
    }
}