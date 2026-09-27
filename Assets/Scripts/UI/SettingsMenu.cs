using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Volume")]
    public Slider volumeSlider;

    [Header("Mouse Sensitivity")]
    public Slider sensitivitySlider;
    public TMP_Text sensitivityValueText;

    [Header("Menus")]
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;

    [Header("Mouse Look")]
    public MouseLook[] mouseLookScripts;

    private float savedVolume;
    private float previousVolume;

    private float savedSensitivity;
    private float previousSensitivity;

    void Start()
    {
        // -------------------------
        // Volume
        // -------------------------

        savedVolume = PlayerPrefs.GetFloat("Volume", 1f);

        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        previousVolume = savedVolume;

        volumeSlider.onValueChanged.AddListener(PreviewVolume);


        // -------------------------
        // Mouse Sensitivity
        // -------------------------

        savedSensitivity = PlayerPrefs.GetFloat("Sensitivity", 50f);

        sensitivitySlider.minValue = 0f;
        sensitivitySlider.maxValue = 100f;
        sensitivitySlider.wholeNumbers = true;

        sensitivitySlider.value = savedSensitivity;

        previousSensitivity = savedSensitivity;

        sensitivitySlider.onValueChanged.AddListener(PreviewSensitivity);

        UpdateSensitivityText(savedSensitivity);

        // Apply saved sensitivity when the game starts
        ApplySensitivity(savedSensitivity);
    }


    // -------------------------
    // Volume
    // -------------------------

    public void PreviewVolume(float volume)
    {
        AudioListener.volume = volume;
    }


    // -------------------------
    // Sensitivity Preview
    // -------------------------

    public void PreviewSensitivity(float sensitivity)
    {
        UpdateSensitivityText(sensitivity);

        ApplySensitivity(sensitivity);
    }


    private void ApplySensitivity(float sliderValue)
    {
        // Convert 0-100 slider into 0-20 actual sensitivity
        float actualSensitivity = sliderValue / 5f;

        foreach (MouseLook mouseLook in mouseLookScripts)
        {
            if (mouseLook != null)
            {
                mouseLook.SetSensitivity(actualSensitivity);
            }
        }
    }


    private void UpdateSensitivityText(float sensitivity)
    {
        sensitivityValueText.text = Mathf.RoundToInt(sensitivity).ToString();
    }


    // -------------------------
    // Apply Button
    // -------------------------

    public void ApplySettings()
    {
        // Save volume
        savedVolume = volumeSlider.value;

        PlayerPrefs.SetFloat("Volume", savedVolume);

        previousVolume = savedVolume;


        // Save sensitivity
        savedSensitivity = sensitivitySlider.value;

        PlayerPrefs.SetFloat("Sensitivity", savedSensitivity);

        previousSensitivity = savedSensitivity;

        PlayerPrefs.Save();
    }


    // -------------------------
    // Back Button
    // -------------------------

    public void Back()
    {
        // Restore volume
        volumeSlider.value = previousVolume;
        AudioListener.volume = previousVolume;


        // Restore sensitivity
        sensitivitySlider.value = previousSensitivity;

        UpdateSensitivityText(previousSensitivity);
        ApplySensitivity(previousSensitivity);


        // Switch back to Pause Menu
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}