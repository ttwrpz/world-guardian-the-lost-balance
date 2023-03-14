using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collectibles", menuName = "Collectibles/Collectible")]
public class CollectibleSO : ScriptableObject
{
    public string id;
    public CollectibleType type;
    public Sprite icon;
    public GameObject prefab;
    public string collectibleName;
    public string description;
    public bool isUnlocked;
    public bool unlockedDate;
}
