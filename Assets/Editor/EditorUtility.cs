using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorUtility : Editor
{

    [MenuItem("Project Utility/Open/Game Folder")]
    static void OpenPersistentDataPath()
    {
        Process.Start("explorer.exe", Path.GetFullPath(Application.persistentDataPath));
    }

    [MenuItem("Project Utility/Open/Worlds Folder")]
    static void OpenWorldSaveWorld()
    {
        Process.Start("explorer.exe", Path.GetFullPath(SaveManager.SavePath));
    }

    [MenuItem("Project Utility/Open/Backups Folder")]
    static void OpenBackupWorld()
    {
        Process.Start("explorer.exe", Path.GetFullPath(SaveManager.BackupPath));
    }

    [MenuItem("Project Utility/Scripts Helper/Find Missing Scripts In Projects")]

    static void FindMissingScriptsInProjectMenuItem()
    {
        string[] prefabPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();


        foreach (string path in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            foreach (Component component in prefab.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    UnityEngine.Debug.Log("Prefab found wiith missing script " + path, prefab);
                    break;
                }
            }
        }
    }

    [MenuItem("Project Utility/Scripts Helper/Find Missing Scripts In Scene")]

    static void FindMissingScriptsInSceneMenuItem()
    {
        foreach (GameObject gameObject in Object.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None))
        {
            foreach (Component component in gameObject.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    UnityEngine.Debug.Log("GameObject found with missing script " + gameObject.name, gameObject);
                    break;
                }
            }
        }
    }
}
