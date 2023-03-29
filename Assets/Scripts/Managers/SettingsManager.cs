using System.IO;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public delegate void OnSettingsLoaded();
    public event OnSettingsLoaded SettingsLoaded;

    private string settingsFilePath;

    public GameSettings Settings;

    [System.Serializable]
    public class GameSettings {
        public float MasterVolume = 1f;
        public float MusicVolume = 1f;
        public float SfxVolume = 1f;

        public int ScreenWidth = 1920;
        public int ScreenHeight = 1080;
        public FullScreenMode ScreenMode = FullScreenMode.FullScreenWindow;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.json");

        LoadSettings();
        CreateSaveFileIfNotExists();
    }

    public void CreateSaveFileIfNotExists()
    {
        if (!File.Exists(settingsFilePath))
        {
            Settings = new GameSettings();
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(Settings, true);
        File.WriteAllText(settingsFilePath, json);
    }

    public void LoadSettings()
    {
        if (File.Exists(settingsFilePath))
        {
            string json = File.ReadAllText(settingsFilePath);
            Settings = JsonUtility.FromJson<GameSettings>(json);
        }
        else
        {
            Settings = new GameSettings();
            SaveSettings();
        }

        ApplySettings();
    }

    private void ApplySettings()
    {
        Screen.SetResolution(Settings.ScreenWidth, Settings.ScreenHeight, Settings.ScreenMode);
        SettingsLoaded?.Invoke();
    }
}