
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

    // =========================================================
    // UPDATE
    // =========================================================

  private void Update()
{
    if (!Input.GetKeyDown(KeyCode.Escape))
        return;

    // If the pause menu is open, resume the game.
    if (GameIsPaused)
    {
        Resume();
        return;
    }

    // If a book is open, close it with Escape.
    if (playerState != null && playerState.IsBook())
    {
        HandBook handBook = FindFirstObjectByType<HandBook>();
        TutBook tutBook = FindFirstObjectByType<TutBook>();

        if (handBook != null &&
            handBook.bookCanvas != null &&
            handBook.bookCanvas.activeSelf)
        {
            handBook.CloseBook();
            return;
        }

        if (tutBook != null &&
            tutBook.bookCanvas != null &&
            tutBook.bookCanvas.activeSelf)
        {
            tutBook.CloseBook();
            return;
        }

        // Don't open the pause menu if the player is in Book state.
        return;
    }

    // Otherwise, open the pause menu.
    Pause();
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

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);

        if (controlsMenuUI != null)
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
        // Cursor.lockState is managed by LockMouse.
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

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);

        if (controlsMenuUI != null)
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
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(true);

        if (controlsMenuUI != null)
            controlsMenuUI.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseSettings()
    {
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);

        if (controlsMenuUI != null)
            controlsMenuUI.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // =========================================================
    // CONTROLS
    // =========================================================

    public void OpenControls()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);

        if (controlsMenuUI != null)
            controlsMenuUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseControls()
    {
        if (controlsMenuUI != null)
            controlsMenuUI.SetActive(false);

        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);

        if (pauseMenuUI != null)
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
