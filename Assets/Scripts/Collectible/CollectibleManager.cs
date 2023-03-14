using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CollectibleManager : MonoBehaviour
{
    public string collectiblePath = Path.Join(Application.persistentDataPath,"/collectibles.dat");

    public Dictionary<CollectibleType, CollectibleSO> collectibles = new();
    public Dictionary<CollectibleType, List<CollectibleSO>> collectedCollectibles = new();

    private void Start()
    {
        LoadCollectibles();
        LoadCollectedCollectibles();
    }

    private void LoadCollectibles()
    {
        CollectibleSO[] collectiblesArray = Resources.LoadAll<CollectibleSO>("Collectibles");

        foreach (CollectibleSO collectible in collectiblesArray)
        {
            if (collectible.prefab != null)
            {
                //Instantiate(collectible.prefab, collectible.spawnPosition, Quaternion.identity);
            }
            collectibles.Add(collectible.type, collectible);
        }
    }

    private void LoadCollectedCollectibles()
    {
        if (File.Exists(collectiblePath))
        {
            BinaryFormatter formatter = new();
            using FileStream file = File.Open(collectiblePath, FileMode.Open);
            collectedCollectibles = (Dictionary<CollectibleType, List<CollectibleSO>>)formatter.Deserialize(file);
        }
        else
        {
            foreach (CollectibleType type in Enum.GetValues(typeof(CollectibleType)))
            {
                collectedCollectibles.Add(type, new List<CollectibleSO>());
            }
        }
    }

    public List<CollectibleSO> GetCollectiblesByType(CollectibleType type)
    {
        List<CollectibleSO> collectiblesOfType = new();

        foreach (CollectibleSO collectible in collectibles.Values)
        {
            if (collectible.type == type)
            {
                collectiblesOfType.Add(collectible);
            }
        }

        return collectiblesOfType;
    }

    public void SaveCollectibles()
    {
        BinaryFormatter formatter = new();
        using FileStream file = File.Create(collectiblePath);
        formatter.Serialize(file, collectibles);
    }

    public void AddCollectible(CollectibleSO collectible)
    {
        if (!collectibles.ContainsKey(collectible.type))
        {
            collectible.isUnlocked = false;
            collectibles.Add(collectible.type, collectible);
            SaveCollectibles();
        }
        else
        {
            Debug.LogWarning($"A collectible with type {collectible.type} already exists!");
        }
    }

    public void RemoveCollectible(CollectibleType type)
    {
        if (collectibles.ContainsKey(type))
        {
            collectibles.Remove(type);
            SaveCollectibles();
        }
        else
        {
            Debug.LogWarning($"No collectible with type {type} exists!");
        }
    }
}
