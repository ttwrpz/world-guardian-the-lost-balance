using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public class SaveManager
{
    public static string _savePath = Application.persistentDataPath + "/saves";
    public static string _backupPath = Application.persistentDataPath + "/backups";

    [MenuItem("World's Guadian/World/Open Worlds Folder")]
    static void OpenWorldSaveWorld()
    {
        Process.Start("explorer.exe", Path.GetFullPath(_savePath));
    }

    [MenuItem("World's Guadian/World/Open Backups Folder")]
    static void OpenBackupWorld()
    {
        Process.Start("explorer.exe", Path.GetFullPath(_backupPath));
    }

    public static void CreateWorld(World world)
    {
        string _initialWorldPath = Path.Join(_savePath, world.worldName);
        string _currentWorldPath = _initialWorldPath;
        int _increment = 0;
        while (Directory.Exists(_currentWorldPath))
        {
            _increment++;
            _currentWorldPath = string.Concat(_initialWorldPath, " (", _increment, ")");
        }
        Directory.CreateDirectory(_currentWorldPath);

        BinaryFormatter formatter = new();
        using FileStream fileStream = new(Path.Join(_currentWorldPath, "level.dat"), FileMode.Create);
        formatter.Serialize(fileStream, world);
    }

    public static string[] LoadWorldListEntry()
    {
        if (!Directory.Exists(_savePath))
        {
            Directory.CreateDirectory(_savePath);
            return new string[] { };
        }

        return Directory.GetDirectories(_savePath);
    }

    public static List<World> LoadWorldDataListEntry()
    {
        string[] directorySaveList = Directory.GetDirectories(_savePath);
        List<World> directorySaveInfo = new();

        foreach (string directorySave in directorySaveList)
        {
            string _saveFile = Path.Join(directorySave, "level.dat");

            if (!File.Exists(_saveFile))
                continue;

            BinaryFormatter formatter = new();
            using FileStream fileStream = new(_saveFile, FileMode.Open);

            World worldSettings = (World)formatter.Deserialize(fileStream);
            worldSettings.worldFolder = Path.GetFileName(directorySave);
            //worldSettings.worldModifiedAt = Directory.GetLastWriteTime(_saveFile); When Save World Only
            directorySaveInfo.Add(worldSettings);
        }

        return directorySaveInfo;
    }

    public static void LoadWorldData(World world)
    {

    }

    public static void SaveWorldData(World worldNewData)
    {
        string _saveFile = Path.Join(_savePath, worldNewData.worldFolder, "level.dat");

        if (!File.Exists(_saveFile))
            return;

        BinaryFormatter formatter = new();
        using FileStream fileStream = new(_saveFile, FileMode.Open);

        World worldData = (World)formatter.Deserialize(fileStream);

        worldData.worldName ??= worldNewData.worldName;
        worldData.worldGameMode = worldNewData.worldGameMode.Equals(worldData.worldGameMode) ? worldData.worldGameMode : worldNewData.worldGameMode;
        worldData.worldDifficulty = worldNewData.worldDifficulty.Equals(worldData.worldDifficulty) ? worldData.worldDifficulty : worldNewData.worldDifficulty;
        worldData.worldModifiedAt = DateTime.Now;

        formatter.Serialize(fileStream, worldNewData);
    }

    public static void DeleteWorld(World world)
    {
        string _worldFolder = Path.Join(_savePath, world.worldFolder);

        if (Directory.Exists(_worldFolder))
        {
            Directory.Delete(_savePath, true);
        }
    }

    public static string BackupWorld(World world)
    {
        if (!Directory.Exists(_backupPath))
            Directory.CreateDirectory(_backupPath);

        string zipName = string.Concat(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), "_", world.worldFolder, ".zip");
        ZipFile.CreateFromDirectory(Path.Join(SaveManager._savePath, world.worldFolder), Path.Join(SaveManager._backupPath, zipName));

        return zipName;
    }
}
