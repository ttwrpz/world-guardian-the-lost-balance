using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager
{
    public static readonly string SavePath = Path.Combine(Application.persistentDataPath, "saves");
    public static readonly string BackupPath = Path.Combine(Application.persistentDataPath, "backups");

    public static void CreateWorld(World world)
    {
        string escapedFileName = new(world.WorldName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
        string initialWorldPath = Path.Combine(SavePath, escapedFileName);
        string currentWorldPath = initialWorldPath;
        int increment = 0;
        while (Directory.Exists(currentWorldPath))
        {
            increment++;
            currentWorldPath = $"{initialWorldPath} ({increment})";
        }
        Directory.CreateDirectory(currentWorldPath);

        BinaryFormatter formatter = new();
        using FileStream fileStream = new(Path.Combine(currentWorldPath, "level.dat"), FileMode.Create);
        formatter.Serialize(fileStream, world);
    }

    public static string[] LoadWorldListEntry()
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
            return Array.Empty<string>();
        }

        return Directory.GetDirectories(SavePath);
    }

    public static List<World> LoadWorldDataListEntry()
    {
        List<World> directorySaveInfo = new();

        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
            return directorySaveInfo;
        }

        string[] directorySaveList = Directory.GetDirectories(SavePath);

        foreach (string directorySave in directorySaveList)
        {
            string saveFile = Path.Combine(directorySave, "level.dat");

            if (!File.Exists(saveFile))
                continue;

            BinaryFormatter formatter = new();
            using FileStream fileStream = new(saveFile, FileMode.Open);

            World worldSettings = (World)formatter.Deserialize(fileStream);
            worldSettings.WorldFolder = Path.GetFileName(directorySave);
            directorySaveInfo.Add(worldSettings);
        }

        return directorySaveInfo;
    }

    public static void LoadWorldData(World world)
    {
        if (world == null)
            throw new ArgumentNullException(nameof(world));

        string saveFile = Path.Combine(SavePath, world.WorldFolder, "level.dat");

        if (!File.Exists(saveFile))
            throw new FileNotFoundException("Save file not found.", saveFile);

        BinaryFormatter formatter = new BinaryFormatter();
        using FileStream fileStream = new(saveFile, FileMode.Open);

        World worldData = (World)formatter.Deserialize(fileStream);

        world.WorldName ??= worldData.WorldName;
        world.WorldGameMode = world.WorldGameMode.Equals(worldData.WorldGameMode) ? worldData.WorldGameMode : world.WorldGameMode;
        world.WorldDifficulty = world.WorldDifficulty.Equals(worldData.WorldDifficulty) ? worldData.WorldDifficulty : world.WorldDifficulty;
        world.WorldModifiedAt = DateTime.Now;
    }

    public static void SaveWorldData(World worldNewData)
    {
        string _saveFile = Path.Combine(SavePath, worldNewData.WorldFolder, "level.dat");

        if (!File.Exists(_saveFile))
            return;

        BinaryFormatter formatter = new();
        using FileStream fileStream = new(_saveFile, FileMode.Open);

        World worldData = (World)formatter.Deserialize(fileStream);

        worldData.WorldName ??= worldNewData.WorldName;
        worldData.WorldGameMode = worldNewData.WorldGameMode.Equals(worldData.WorldGameMode) ? worldData.WorldGameMode : worldNewData.WorldGameMode;
        worldData.WorldDifficulty = worldNewData.WorldDifficulty.Equals(worldData.WorldDifficulty) ? worldData.WorldDifficulty : worldNewData.WorldDifficulty;
        worldData.WorldModifiedAt = DateTime.Now;

        fileStream.Seek(0, SeekOrigin.Begin);
        formatter.Serialize(fileStream, worldNewData);
        fileStream.SetLength(fileStream.Position);
        fileStream.Flush();
    }

    private static void CreateWorldSave(World world, WorldSave worldSave)
    {
        BinaryFormatter formatter = new();
        using FileStream fileStream = new(Path.Combine(world.WorldFolder, "save.dat"), FileMode.Create);
        formatter.Serialize(fileStream, worldSave);
    }

    public static WorldSave LoadWorldSave(World world)
    {
        string _saveFile = Path.Combine(SavePath, world.WorldFolder, "save.dat");

        BinaryFormatter formatter = new();
        using FileStream fileStream = new(_saveFile, FileMode.Open);

        return (WorldSave)formatter.Deserialize(fileStream);
    }

    public static void SaveWorld(World world, WorldSave worldSave)
    {
        string _saveFile = Path.Combine(SavePath, world.WorldFolder, "save.dat");

        if (!File.Exists(_saveFile))
        {
            CreateWorldSave(world, worldSave);
        }

        BinaryFormatter formatter = new();
        using FileStream fileStream = new(_saveFile, FileMode.Open);

        WorldSave save = (WorldSave)formatter.Deserialize(fileStream);

        fileStream.Seek(0, SeekOrigin.Begin);
        formatter.Serialize(fileStream, save);
        fileStream.SetLength(fileStream.Position);
        fileStream.Flush();
    }

    public static void DeleteWorld(World world)
    {
        string worldFolder = Path.Combine(SavePath, world.WorldFolder);

        if (Directory.Exists(worldFolder))
        {
            Directory.Delete(worldFolder, true);
        }
    }

    public static string BackupWorld(World world)
    {
        if (!Directory.Exists(BackupPath))
        {
            Directory.CreateDirectory(BackupPath);
        }

        string zipName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{world.WorldFolder}.zip";
        string sourcePath = Path.Combine(SavePath, world.WorldFolder);
        string destinationPath = Path.Combine(BackupPath, zipName);

        ZipFile.CreateFromDirectory(sourcePath, destinationPath);

        return zipName;
    }

}
