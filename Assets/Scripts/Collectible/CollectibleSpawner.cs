using UnityEngine;
using System.Collections.Generic;

public class CollectibleSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public Collectible.CollectibleType collectibleType;

    CollectibleManager collectibleManager;

    private void Start()
    {
        collectibleManager = FindFirstObjectByType<CollectibleManager>();
        if (collectibleManager == null)
        {
            Debug.LogError("AchievementManager not found in the scene.");
            return;
        }

        SpawnCollectibles();
    }

    private void SpawnCollectibles()
    {
        List<Collectible> collectibles = collectibleManager.LoadCollectiblesByType(collectibleType);

        foreach (Collectible collectible in collectibles)
        {
            if (!collectibleManager.IsCollectibleUnlocked(collectible.id))
            {
                Transform spawnPoint = GetRandomSpawnPoint();
                GameObject collectiblePrefab = Resources.Load<GameObject>($"Prefabs/Collectibles/{collectible.id}");
                GameObject spawnedCollectible = Instantiate(collectiblePrefab, spawnPoint.position, spawnPoint.rotation);
                spawnedCollectible.GetComponent<CollectibleController>().collectible = collectible;
            }
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        int index = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[index];
        spawnPoints.RemoveAt(index);
        return spawnPoint;
    }
}