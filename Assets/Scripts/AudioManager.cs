using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public float masterVolume = 1.0f;

    public AudioSource sfxSource;
    public AudioSource musicSource;

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
        }

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SettingsLoaded += UpdateVolumeSettings;
            UpdateVolumeSettings();
        }
    }

    public AudioClip[] sfxClips;
    public AudioClip[] musicClips;

    public void PlaySFX(int clipIndex, float volume = 1.0f)
    {
        sfxSource.PlayOneShot(sfxClips[clipIndex], volume * masterVolume);
    }

    public void PlayMusic(int clipIndex, float volume = 1.0f)
    {
        musicSource.clip = musicClips[clipIndex];
        musicSource.volume = volume * masterVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        musicSource.volume *= masterVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume * masterVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume * masterVolume;
    }

    private void UpdateVolumeSettings()
    {
        SetMasterVolume(SettingsManager.Instance.Settings.MasterVolume);
        SetMusicVolume(SettingsManager.Instance.Settings.MusicVolume);
        SetSfxVolume(SettingsManager.Instance.Settings.SfxVolume);
    }

}
