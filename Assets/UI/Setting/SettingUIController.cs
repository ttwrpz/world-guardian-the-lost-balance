using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private Button _backButton;

    private GroupBox _audioGroup;
    public Slider masterVolumeSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    private GroupBox _videoGroup;
    public DropdownField windowModeDropdown;
    public DropdownField resolutionDropdown;

    private Resolution[] resolutions;

    private void Awake()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SettingsLoaded += LoadSavedSettings;
        }
    }


    private void OnEnable()
    {
        GetUIElements();
        AttachEventHandlers();
        LoadSavedSettings();
        LoadSavedWindowMode();
        LoadSavedResolution();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _backButton = _root.Q<Button>("BackButton");

        _audioGroup = _root.Q<GroupBox>("Audio");

        masterVolumeSlider = _audioGroup.Q<Slider>("MasterSlider");
        sfxSlider = _audioGroup.Q<Slider>("SfxSlider");
        musicSlider = _audioGroup.Q<Slider>("MusicSlider");

        _videoGroup = _root.Q<GroupBox>("Video");

        windowModeDropdown = _videoGroup.Q<DropdownField>("WindowModeDropdown");
        SetupWindowModeDropdown();

        resolutionDropdown = _videoGroup.Q<DropdownField>("ResolutionDropdown");
        SetupResolutionDropdown();
    }

    private void AttachEventHandlers()
    {
        _backButton.clicked += UIManager.BackToMainMenuUI;

        masterVolumeSlider.RegisterValueChangedCallback(evt => AudioManager.Instance.SetMasterVolume(evt.newValue));
        sfxSlider.RegisterValueChangedCallback(evt => AudioManager.Instance.SetSfxVolume(evt.newValue));
        musicSlider.RegisterValueChangedCallback(evt => AudioManager.Instance.SetMusicVolume(evt.newValue));
    }

    private void LoadSavedSettings()
    {
        if (SettingsManager.Instance != null && masterVolumeSlider != null && sfxSlider != null && musicSlider != null)
        {
            masterVolumeSlider.value = SettingsManager.Instance.Settings.MasterVolume;
            sfxSlider.value = SettingsManager.Instance.Settings.MusicVolume;
            musicSlider.value = SettingsManager.Instance.Settings.SfxVolume;
        }
    }


    private void SetupWindowModeDropdown()
    {
        List<string> windowModeOptions = new() { "Fullscreen", "Borderless", "Windowed" };
        windowModeDropdown.choices = windowModeOptions;

        windowModeDropdown.RegisterValueChangedCallback(evt =>
        {
            SetWindowMode(windowModeDropdown.choices.IndexOf(evt.newValue));
        });
    }

    private void LoadSavedWindowMode()
    {
        if (SettingsManager.Instance != null && windowModeDropdown != null)
        {
            int currentWindowModeIndex = (SettingsManager.Instance.Settings.ScreenMode == FullScreenMode.ExclusiveFullScreen)
                ? 0
                : (SettingsManager.Instance.Settings.ScreenMode == FullScreenMode.FullScreenWindow)
                    ? 1
                    : 2;

            windowModeDropdown.index = currentWindowModeIndex;
        }
    }

    private void SetupResolutionDropdown()
    {
        resolutions = Screen.resolutions
            .Where(res => (res.width >= 1000) && (IsAspectRatio16By9(res) || IsAspectRatio16By10(res)))
            .GroupBy(resolution => new { resolution.width, resolution.height })
            .Select(group => group.First())
            .ToArray();

        resolutionDropdown.choices = resolutions.Select(resolution => $"{resolution.width} x {resolution.height}").ToList();

        resolutionDropdown.RegisterValueChangedCallback(evt =>
        {
            SetResolution(resolutionDropdown.choices.IndexOf(evt.newValue));
        });
    }

    private void LoadSavedResolution()
    {
        if (SettingsManager.Instance != null && resolutionDropdown != null)
        {
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == SettingsManager.Instance.Settings.ScreenWidth &&
                    resolutions[i].height == SettingsManager.Instance.Settings.ScreenHeight)
                {
                    currentResolutionIndex = i;
                    break;
                }
            }

            resolutionDropdown.index = currentResolutionIndex;
        }
    }


    private bool IsAspectRatio16By9(Resolution resolution)
    {
        float aspectRatio = (float)resolution.width / resolution.height;
        return Mathf.Approximately(aspectRatio, 16f / 9f);
    }

    private bool IsAspectRatio16By10(Resolution resolution)
    {
        float aspectRatio = (float)resolution.width / resolution.height;
        return Mathf.Approximately(aspectRatio, 16f / 10f);
    }

    public void SetWindowMode(int modeIndex)
    {
        FullScreenMode mode;

        switch (modeIndex)
        {
            case 0:
                mode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                mode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                mode = FullScreenMode.Windowed;
                break;
            default:
                mode = FullScreenMode.FullScreenWindow;
                break;
        }

        Screen.fullScreenMode = mode;

        SettingsManager.Instance.Settings.ScreenMode = mode;

        SettingsManager.Instance.SaveSettings();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);

        SettingsManager.Instance.Settings.ScreenWidth = resolution.width;
        SettingsManager.Instance.Settings.ScreenHeight = resolution.height;

        SettingsManager.Instance.SaveSettings();
    }
}
