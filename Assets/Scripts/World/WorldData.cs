using UnityEngine;

[CreateAssetMenu]
public class WorldData : ScriptableObject
{
    public void Initialize(World world)
    {
        WorldName = world.WorldName;
        WorldSeed = world.WorldSeed;
        WorldGameMode = world.WorldGameMode;
        WorldDifficulty = world.WorldDifficulty;
        WorldFolder = world.WorldFolder;
    }

    public void ClearData()
    {
        WorldName = "";
        WorldSeed = "";
        WorldGameMode = 0;
        WorldDifficulty = 0;
        WorldFolder = "";
    }

    [SerializeField]
    private string _worldName;

    public string WorldName
    {
        get { return _worldName; }
        set { _worldName = value; }
    }

    [SerializeField]
    private string _worldSeed;

    public string WorldSeed
    {
        get { return _worldSeed; }
        set { _worldSeed = value; }
    }

    [SerializeField]
    private World.GameMode _worldGameMode;

    public World.GameMode WorldGameMode
    {
        get { return _worldGameMode; }
        set { _worldGameMode = value; }
    }

    [SerializeField]
    private World.Difficulty _worldDifficulty;

    public World.Difficulty WorldDifficulty
    {
        get { return _worldDifficulty; }
        set { _worldDifficulty = value; }
    }

    [SerializeField]
    private string _worldFolder;

    public string WorldFolder
    {
        get { return _worldFolder; }
        set { _worldFolder = value; }
    }

}
