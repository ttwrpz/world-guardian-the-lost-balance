using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleUIEventHandlers : MonoBehaviour
{
    public static async void onEndingButtonClicked()
    {
        await UIManager.LoadSceneAsync("Collectible/Endings");
    }

    public static async void onItemButtonClicked()
    {
        await UIManager.LoadSceneAsync("Collectible/Items");
    }
    public static async void onLandmarkButtonClicked()
    {
        await UIManager.LoadSceneAsync("Collectible/Landmarks");
    }

    public static async void onLoreButtonClicked()
    {
        await UIManager.LoadSceneAsync("Collectible/Lores");
    }

    public static async void onBackToCollectibleButtonClicked()
    {
        await UIManager.LoadSceneAsync("Collectible/Collectible");
    }
}
