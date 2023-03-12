using System;

[Serializable]
public class World
{
    public string worldName;
    public string worldSeed;

    public enum WorldGameMode { Story, Sandbox };
    public WorldGameMode worldGameMode;

    public enum WorldDifficulty { Easy, Medium, Hard };
    public WorldDifficulty worldDifficulty;

    public enum WorldSize { Small, Medium, Large };
    public WorldSize worldSize;

    public string worldFolder;
    public string worldVersion;
    public DateTime worldCreatedAt;
    public DateTime worldModifiedAt;

    public static string ConvertFormat(string seed)
    {
        return seed;
    }
}
