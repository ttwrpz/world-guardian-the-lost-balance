using System;
using System.Linq;

[Serializable]
public class World
{
    public string WorldName { get; set; }
    public int WorldSeed { get; set; }

    public enum GameMode { Story, Sandbox };
    public GameMode WorldGameMode { get; set; }

    public enum Difficulty { Easy, Medium, Hard };
    public Difficulty WorldDifficulty {get; set; }

    public string WorldFolder { get; set; }
    public string WorldVersion { get; set; }
    public DateTime WorldCreatedAt { get; set; }
    public DateTime WorldModifiedAt { get; set; }

    public static int GenerateRandomSeed()
    {
        return UnityEngine.Random.Range(-10000, 10000);
    }

    public static int ConvertSeedFormat(string seed)
    {
        return seed.Select(c => (int)c).Sum() % 20001 - 10000;
    }
}
