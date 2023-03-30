using System;
using System.Collections.Generic;

[System.Serializable]
public class CollectibleData
{
    public List<CollectibleInfo> unlockedCollectibles;

    public CollectibleData()
    {
        unlockedCollectibles = new List<CollectibleInfo>();
    }

    [System.Serializable]
    public class CollectibleInfo
    {
        public string id;
        public string collectedDateString;
    }
}