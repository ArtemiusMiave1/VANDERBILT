using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
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

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;

        // Hide and lock cursor during gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);

        Time.timeScale = 0f;
        GameIsPaused = true;

        // Show and unlock cursor while paused
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Called by the Settings button
    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);

        // Make sure cursor stays available
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Called by the Back button in Settings
    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }
}