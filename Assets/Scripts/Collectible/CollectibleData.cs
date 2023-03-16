using System.Collections.Generic;

[System.Serializable]
public class CollectibleData
{
    public List<string> unlockedCollectibles;

    public CollectibleData()
    {
        unlockedCollectibles = new List<string>();
    }
}