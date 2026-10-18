
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("Menu UI")]
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject controlsMenuUI;

    [Header("Player")]
    public PlayerState playerState;

    private void Start()
    {
        if (playerState == null)
        {
            playerState = FindFirstObjectByType<PlayerState>();
        }

        GameIsPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // =========================================================
    // RESUME
    // =========================================================

    public void Resume()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.ResumeGame();
        }

        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        controlsMenuUI.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        GameIsPaused = false;

        // Return player to gameplay.
        if (playerState != null)
        {
            playerState.SetState(PlayerState.State.Gameplay);
        }

        Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // =========================================================
    // PAUSE
    // =========================================================

    public void Pause()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.PauseGame();
        }

        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        controlsMenuUI.SetActive(false);

        Time.timeScale = 0f;
        AudioListener.pause = true;
        GameIsPaused = true;

        // Change player to menu state.
        if (playerState != null)
        {
            playerState.SetState(PlayerState.State.Menu);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // =========================================================
    // SETTINGS
    // =========================================================

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
        controlsMenuUI.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        controlsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // =========================================================
    // CONTROLS
    // =========================================================

    public void OpenControls()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        controlsMenuUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseControls()
    {
        controlsMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // =========================================================
    // MAIN MENU
    // =========================================================

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        GameIsPaused = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }
}
