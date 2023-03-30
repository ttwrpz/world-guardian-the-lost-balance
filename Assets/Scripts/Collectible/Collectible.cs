using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Collectible", menuName = "Collectible")]
public class Collectible : ScriptableObject
{
    public enum CollectibleType
    {
        Endings,
        Items,
        Landmarks,
        Lores
    }

    public string id;
    public CollectibleType type;
    public Sprite icon;
    public string collectibleName;
    public string description;
    public bool isCollected;
    public DateTime collectedDate;

}
