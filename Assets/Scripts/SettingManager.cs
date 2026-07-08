using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using VHS;

public class SettingsManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;

    [Header("Scene Type")]
    public bool isMainMenuScene = false;

    [Header("Mouse Sensitivity")]
    public Slider sensitivitySlider;

    [Tooltip("Leave empty in the Main Menu scene. Assign the active CameraController in the game scene.")]
    public CameraController cameraController;

    [Header("Master Volume")]
    public Slider masterVolumeSlider;

    [Header("Brightness")]
    public Slider brightnessSlider;
    public Image brightnessOverlay;

    [Header("Resolution")]
    public TMP_Dropdown resolutionDropdown;

    [Header("Fullscreen")]
    public Toggle fullscreenToggle;

    private readonly List<Vector2Int> resolutions = new List<Vector2Int>();
    private bool loadingSettings = false;

    private const string SensitivityKey = "MouseSensitivity";
    private const string MasterVolumeKey = "MasterVolume";
    private const string BrightnessKey = "Brightness";
    private const string ResolutionKey = "ResolutionIndex";
    private const string FullscreenKey = "Fullscreen";

    private void Start()
    {
        loadingSettings = true;

        SetupSliderRanges();
        SetupResolutions();
        LoadSettings();

        loadingSettings = false;

        AddListeners();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

    private void SetupSliderRanges()
    {
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.2f;
            sensitivitySlider.maxValue = 3f;
            sensitivitySlider.wholeNumbers = false;
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.wholeNumbers = false;
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.minValue = 0f;
            brightnessSlider.maxValue = 1f;
            brightnessSlider.wholeNumbers = false;
        }
    }

    private void AddListeners()
    {
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);

        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.AddListener(SetBrightness);

        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(SetResolution);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void RemoveListeners()
    {
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.RemoveListener(SetSensitivity);

        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);

        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.RemoveListener(SetBrightness);

        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.RemoveListener(SetResolution);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
    }

    private void SetupResolutions()
    {
        if (resolutionDropdown == null)
            return;

        resolutionDropdown.ClearOptions();
        resolutions.Clear();

        resolutions.Add(new Vector2Int(1280, 720));
        resolutions.Add(new Vector2Int(1600, 900));
        resolutions.Add(new Vector2Int(1920, 1080));
        resolutions.Add(new Vector2Int(2560, 1440));

        List<string> options = new List<string>();

        foreach (Vector2Int resolution in resolutions)
            options.Add(resolution.x + " x " + resolution.y);

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
    }

    private void LoadSettings()
    {
        float sensitivity = Mathf.Clamp(
            PlayerPrefs.GetFloat(SensitivityKey, 1f),
            0.2f,
            3f
        );

        float volume = Mathf.Clamp01(
            PlayerPrefs.GetFloat(MasterVolumeKey, 1f)
        );

        float brightness = Mathf.Clamp01(
            PlayerPrefs.GetFloat(BrightnessKey, 0.5f)
        );

        int resolutionIndex = PlayerPrefs.GetInt(
            ResolutionKey,
            GetDefaultResolutionIndex()
        );

        bool fullscreen = PlayerPrefs.GetInt(
            FullscreenKey,
            1
        ) == 1;

        if (sensitivitySlider != null)
            sensitivitySlider.SetValueWithoutNotify(sensitivity);

        if (masterVolumeSlider != null)
            masterVolumeSlider.SetValueWithoutNotify(volume);

        if (brightnessSlider != null)
            brightnessSlider.SetValueWithoutNotify(brightness);

        if (resolutionDropdown != null && resolutions.Count > 0)
        {
            resolutionIndex = Mathf.Clamp(
                resolutionIndex,
                0,
                resolutions.Count - 1
            );

            resolutionDropdown.SetValueWithoutNotify(resolutionIndex);
            resolutionDropdown.RefreshShownValue();
        }

        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        // In the game scene this directly feeds the saved value to the camera.
        if (cameraController != null)
            cameraController.SetSensitivityMultiplier(sensitivity);

        AudioListener.volume = volume;
        ApplyBrightness(brightness);
        ApplyResolution(resolutionIndex);
        Screen.fullScreen = fullscreen;
    }

    public void SetSensitivity(float value)
    {
        if (loadingSettings)
            return;

        value = Mathf.Clamp(value, 0.2f, 3f);

        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();

        // Direct live connection in the game scene.
        if (cameraController != null)
            cameraController.SetSensitivityMultiplier(value);
    }

    public void SetMasterVolume(float value)
    {
        if (loadingSettings)
            return;

        value = Mathf.Clamp01(value);

        AudioListener.volume = value;

        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();
    }

    public void SetBrightness(float value)
    {
        if (loadingSettings)
            return;

        value = Mathf.Clamp01(value);

        ApplyBrightness(value);

        PlayerPrefs.SetFloat(BrightnessKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyBrightness(float value)
    {
        if (brightnessOverlay == null)
            return;

        Color overlayColor = brightnessOverlay.color;
        overlayColor.a = Mathf.Lerp(0.65f, 0f, value);
        brightnessOverlay.color = overlayColor;
    }

    public void SetResolution(int index)
    {
        if (loadingSettings)
            return;

        PlayerPrefs.SetInt(ResolutionKey, index);
        PlayerPrefs.Save();

        if (resolutionDropdown != null)
            resolutionDropdown.Hide();

        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        StartCoroutine(ApplyResolutionDelayed(index));
    }

    private IEnumerator ApplyResolutionDelayed(int index)
    {
        yield return new WaitForSecondsRealtime(0.15f);

        ApplyResolution(index);
    }

    private void ApplyResolution(int index)
    {
        if (resolutions.Count == 0)
            return;

        index = Mathf.Clamp(index, 0, resolutions.Count - 1);

        Vector2Int resolution = resolutions[index];

        Screen.SetResolution(
            resolution.x,
            resolution.y,
            Screen.fullScreen
        );
    }

    public void SetFullscreen(bool fullscreen)
    {
        if (loadingSettings)
            return;

        Screen.fullScreen = fullscreen;

        PlayerPrefs.SetInt(
            FullscreenKey,
            fullscreen ? 1 : 0
        );

        PlayerPrefs.Save();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (isMainMenuScene)
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);
        }
        else
        {
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(true);
        }
    }

    private int GetDefaultResolutionIndex()
    {
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].x == Screen.width &&
                resolutions[i].y == Screen.height)
            {
                return i;
            }
        }

        return 2;
    }
}
