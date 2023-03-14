using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public CollectibleSO collectibleSO;
    public Transform spawnPoint;

    private void Start()
    {
        SpawnCollectible();
    }

    private void SpawnCollectible()
    {
        GameObject prefab = collectibleSO.prefab;
        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
    }
}
