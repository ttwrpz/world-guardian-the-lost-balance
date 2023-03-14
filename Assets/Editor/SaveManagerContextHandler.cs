using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveManagerContextHandler
{
    [MenuItem("World's Guadian/World/Open Worlds Folder")]
    static void OpenWorldSaveWorld()
    {
        Process.Start("explorer.exe", Path.GetFullPath(SaveManager.SavePath));
    }

    [MenuItem("World's Guadian/World/Open Backups Folder")]
    static void OpenBackupWorld()
    {
        Process.Start("explorer.exe", Path.GetFullPath(SaveManager.BackupPath));
    }
}
