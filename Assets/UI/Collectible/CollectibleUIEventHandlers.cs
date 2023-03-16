using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleUIEventHandlers : MonoBehaviour
{
    public static async void onEndingButtonClicked()
    {
        await UIController.LoadSceneAsync("Collectible/Endings");
    }

    public static async void onItemButtonClicked()
    {
        await UIController.LoadSceneAsync("Collectible/Items");
    }
    public static async void onLandmarkButtonClicked()
    {
        await UIController.LoadSceneAsync("Collectible/Landmarks");
    }

    public static async void onLoreButtonClicked()
    {
        await UIController.LoadSceneAsync("Collectible/Lores");
    }

    public static async void onBackToCollectibleButtonClicked()
    {
        await UIController.LoadSceneAsync("Collectible/Collectible");
    }
}
