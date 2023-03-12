using UnityEngine;

[CreateAssetMenu]
public class WorldData : ScriptableObject
{
    public void Initialize(World world)
    {
        WorldName = world.worldName;
        WorldSeed = world.worldSeed;
        WorldGameMode = world.worldGameMode;
        WorldDifficulty = world.worldDifficulty;
        WorldSize = world.worldSize;
        WorldFolder = world.worldFolder;
    }

    public void ClearData()
    {
        WorldName = "";
        WorldSeed = "";
        WorldGameMode = 0;
        WorldDifficulty = 0;
        WorldSize = 0;
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
    private World.WorldGameMode _worldGameMode;

    public World.WorldGameMode WorldGameMode
    {
        get { return _worldGameMode; }
        set { _worldGameMode = value; }
    }

    [SerializeField]
    private World.WorldDifficulty _worldDifficulty;

    public World.WorldDifficulty WorldDifficulty
    {
        get { return _worldDifficulty; }
        set { _worldDifficulty = value; }
    }

    [SerializeField]
    private World.WorldSize _worldSize;

    public World.WorldSize WorldSize
    {
        get { return _worldSize; }
        set { _worldSize = value; }
    }

    [SerializeField]
    private string _worldFolder;

    public string WorldFolder
    {
        get { return _worldFolder; }
        set { _worldFolder = value; }
    }

}
