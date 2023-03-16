using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
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

    public static async void BackToMainUI()
    {
        await LoadSceneAsync("Main/Main");
    }
}
