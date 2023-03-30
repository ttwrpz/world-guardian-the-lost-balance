using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    private TimeManager timeManager;

    public CollectibleManager collectibleManager;

    public GameObject collectibleSpritePrefab;
    public float itemSpawnProbability = 0.2f;
    public float collectibleRotationSpeed = 10.0f;

    private void Awake()
    {
        timeManager = gameManager.GetComponent<TimeManager>();
    }

    void Start()
    {
        timeManager.MonthElapsed += OnMonthElapsed;
    }

    void OnDestroy()
    {
        timeManager.MonthElapsed -= OnMonthElapsed;
    }

    void OnMonthElapsed()
    {
        float randomValue = UnityEngine.Random.value;
        Collectible.CollectibleType type = randomValue < 0.02f ? Collectible.CollectibleType.Lores : Collectible.CollectibleType.Items;

        List<Collectible> collectibles = collectibleManager.LoadCollectiblesByType(type);
        if (collectibles.Count > 0 && Random.value < itemSpawnProbability)
        {
            Collectible chosenCollectible = collectibles[Random.Range(0, collectibles.Count)];
            Vector3 spawnPosition = GetRandomTerrainPosition();
            GameObject collectibleSprite = Instantiate(collectibleSpritePrefab, spawnPosition, Quaternion.identity);

            if (chosenCollectible.icon)
            {
                collectibleSprite.GetComponent<SpriteRenderer>().sprite = chosenCollectible.icon;
            }
            collectibleSprite.GetComponent<CollectibleCollectHandler>().collectibleId = chosenCollectible.id;
            collectibleSprite.AddComponent<CollectibleRotate>().rotationSpeed = collectibleRotationSpeed;
            collectibleSprite.transform.parent = collectibleManager.transform;
        }
    }

    Vector3 GetRandomTerrainPosition()
    {
        MeshFilter[] meshFilters = FindObjectsByType<MeshFilter>(FindObjectsSortMode.None);
        List<MeshFilter> terrainMeshFilters = new List<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.GetComponent<MeshCollider>() != null)
            {
                terrainMeshFilters.Add(meshFilter);
            }
        }

        MeshFilter randomMeshFilter = terrainMeshFilters[Random.Range(0, terrainMeshFilters.Count)];

        Vector3 randomPoint = randomMeshFilter.mesh.vertices[Random.Range(0, randomMeshFilter.mesh.vertices.Length)];
        randomPoint = randomMeshFilter.transform.TransformPoint(randomPoint);

        float distanceAboveTerrain = 5f;
        float terrainHeight = randomMeshFilter.transform.position.y;
        if (randomPoint.y < terrainHeight + distanceAboveTerrain)
        {
            randomPoint.y = terrainHeight + distanceAboveTerrain;
        }

        return randomPoint;
    }
}

public class CollectibleRotate : MonoBehaviour
{
    public float rotationSpeed = 10.0f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
