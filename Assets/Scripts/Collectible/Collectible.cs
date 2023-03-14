using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public CollectibleSO collectibleSO;

    private CollectibleType type;

    private void Start()
    {
        type = collectibleSO.type;
    }

    private void OnMouseDown()
    {
        Collect();
    }

    private void Collect()
    {
        // Mark collectible as collected
        collectibleSO.isUnlocked = true;

        // Save updated collectible list to a file
        FindAnyObjectByType<CollectibleManager>().SaveCollectibles();

        // Display a message to the player
        Debug.Log("You collected " + collectibleSO.name + "!");
    }
}
