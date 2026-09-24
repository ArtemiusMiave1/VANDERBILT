using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;

    private float savedVolume;
    private float previousVolume;

    void Start()
    {
        // Load the saved volume
        savedVolume = PlayerPrefs.GetFloat("Volume", 1f);

        // Set the slider
        volumeSlider.value = savedVolume;

        // Set the actual game volume
        AudioListener.volume = savedVolume;

        // Remember the saved volume
        previousVolume = savedVolume;

        // Listen for slider changes
        volumeSlider.onValueChanged.AddListener(PreviewVolume);
    }

    // Changes the volume temporarily
    public void PreviewVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // Called by the Apply button
    public void ApplySettings()
    {
        savedVolume = volumeSlider.value;

        // Save the new volume
        PlayerPrefs.SetFloat("Volume", savedVolume);
        PlayerPrefs.Save();

        // This is now the volume we return to if Back is pressed
        previousVolume = savedVolume;
    }

    // Called by the Back button
    public void Back()
    {
        // Restore the previously saved volume
        volumeSlider.value = previousVolume;
        AudioListener.volume = previousVolume;

        // Switch back to the Pause Menu
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}
