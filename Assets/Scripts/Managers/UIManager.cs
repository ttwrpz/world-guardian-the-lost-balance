using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class UIManager
{
    public static async Task LoadSceneAsync(string path)
    {
        try
        {
            await SceneManager.LoadSceneAsync($"Assets/Scenes/{path}.unity");
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while trying to load the gameplay scene: {ex.Message}");
        }
    }

    public static async void BackToSingleplayerUI()
    {
        await LoadSceneAsync("Singleplayer/Singleplayer");
    }

    public static async void BackToAchievementUI()
    {
        await LoadSceneAsync("Achievement/Achievement");
    }

    public static async void BackToCollectibleUI()
    {
        await LoadSceneAsync("Collectible/Collectible");
    }

    public static async void BackToSettingsUI()
    {
        await LoadSceneAsync("Settings/Collectible");
    }

    public static async void BackToMainMenuUI()
    {
        await LoadSceneAsync("Main/Main");
    }
}
