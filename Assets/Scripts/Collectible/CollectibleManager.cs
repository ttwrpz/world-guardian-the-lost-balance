using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    private CollectibleData collectibleData;
    private string collectibleSavePath;

    private void Awake()
    {
        collectibleSavePath = Path.Combine(Application.persistentDataPath, "collectibles.json");
        CreateSaveFileIfNotExists();
        LoadCollectibleData();
        LoadAllCollectibles();
    }

    public void CreateSaveFileIfNotExists()
    {
        if (!File.Exists(collectibleSavePath))
        {
            collectibleData = new();
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

    public void LoadAllCollectibles()
    {
        foreach (Collectible.CollectibleType type in Enum.GetValues(typeof(Collectible.CollectibleType)))
        {
            List<Collectible> collectibles = LoadCollectiblesByType(type);

            foreach (Collectible collectible in collectibles)
            {
                if (IsCollectibleUnlocked(collectible.id))
                {
                    collectible.isCollected = true;
                    collectible.collectedDate = GetCollectibleCollectedDate(collectible.id);
                }
            }
        }
    }

    public List<Collectible> LoadCollectiblesByType(Collectible.CollectibleType type)
    {
        return new List<Collectible>(Resources.LoadAll<Collectible>($"GameData/Collectibles/{type}"));
    }

    public int GetCollectedCountByType(Collectible.CollectibleType type)
    {
        List<Collectible> collectibles = LoadCollectiblesByType(type);
        return collectibles.Count(c => c.isCollected);
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
        if (!IsCollectibleUnlocked(id))
        {
            CollectibleData.CollectibleInfo newCollectible = new CollectibleData.CollectibleInfo
            {
                id = id,
                collectedDateString = DateTime.Now.ToString("o")
            };
            collectibleData.unlockedCollectibles.Add(newCollectible);
            SaveCollectibleData();
        }
    }

    public bool IsCollectibleUnlocked(string id)
    {
        return collectibleData?.unlockedCollectibles.Any(c => c.id == id) ?? false;
    }

    public DateTime GetCollectibleCollectedDate(string id)
    {
        string collectedDateString = collectibleData?.unlockedCollectibles.FirstOrDefault(c => c.id == id)?.collectedDateString;

        if (!string.IsNullOrEmpty(collectedDateString))
        {
            return DateTime.Parse(collectedDateString, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        return DateTime.MinValue;
    }

    public List<Collectible> GetUnlockedCollectibles()
    {
        List<Collectible> unlockedCollectibles = new List<Collectible>();

        foreach (Collectible.CollectibleType type in Enum.GetValues(typeof(Collectible.CollectibleType)))
        {
            List<Collectible> collectibles = LoadCollectiblesByType(type);
            unlockedCollectibles.AddRange(collectibles.Where(c => c.isCollected));
        }

        return unlockedCollectibles;
    }

}
