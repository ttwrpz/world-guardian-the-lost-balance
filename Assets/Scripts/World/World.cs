using System;

[Serializable]
public class World
{
    public string WorldName { get; set; }
    public string WorldSeed { get; set; }

    public enum GameMode { Story, Sandbox };
    public GameMode WorldGameMode { get; set; }

    public enum Difficulty { Easy, Medium, Hard };
    public Difficulty WorldDifficulty {get; set; }

    public string WorldFolder { get; set; }
    public string WorldVersion { get; set; }
    public DateTime WorldCreatedAt { get; set; }
    public DateTime WorldModifiedAt { get; set; }

    public static string GenerateRandomSeed()
    {
        return "";
    }

    public static string ConvertFormat(string seed)
    {
        return seed;
    }
}
