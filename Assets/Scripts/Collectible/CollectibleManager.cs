using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class CollectibleManager : MonoBehaviour
{
    private CollectibleData collectibleData;
    private string collectibleSavePath;

    private void Awake()
    {
        collectibleSavePath = Path.Combine(Application.persistentDataPath, "collectibles.json");
        LoadCollectibleData();
        CreateSaveFileIfNotExists();
    }

    public void CreateSaveFileIfNotExists()
    {
        if (!File.Exists(collectibleSavePath))
        {
            collectibleData = new()
            {
                unlockedCollectibles = new List<string>()
            };
            SaveCollectibleData();
        }
    }

    public void LoadCollectibleData()
    {
        if (File.Exists(collectibleSavePath))
        {
            string json = File.ReadAllText(collectibleSavePath);
            collectibleData = JsonUtility.FromJson<CollectibleData>(json);
        }
        else
        {
            collectibleData = new CollectibleData();
        }
    }

    public List<Collectible> LoadCollectiblesByType(Collectible.CollectibleType type)
    {
        return new List<Collectible>(Resources.LoadAll<Collectible>($"GameData/Collectibles/{type}"));
    }

    public int GetCollectedCountByType(Collectible.CollectibleType type)
    {
        List<Collectible> collectibles = LoadCollectiblesByType(type);
        return collectibles.Count(c => IsCollectibleUnlocked(c.id));
    }

    public int GetTotalCountByType(Collectible.CollectibleType type)
    {
        List<Collectible> collectibles = LoadCollectiblesByType(type);
        return collectibles.Count;
    }

    public void SaveCollectibleData()
    {
        string json = JsonUtility.ToJson(collectibleData);
        File.WriteAllText(collectibleSavePath, json);
    }

    public void UnlockCollectible(string id)
    {
        if (!collectibleData.unlockedCollectibles.Contains(id))
        {
            collectibleData.unlockedCollectibles.Add(id);
            SaveCollectibleData();
        }
    }

    public bool IsCollectibleUnlocked(string id)
    {
        return collectibleData?.unlockedCollectibles.Contains(id) ?? false;
    }
}