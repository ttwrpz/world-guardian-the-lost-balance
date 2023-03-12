using System.Linq;
using UnityEditor;
using UnityEngine;

public static class FindMissingScripts
{
    [MenuItem("World's Guadian/Scripts Helper/Find Missing Scripts In Projects")]

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
                    Debug.Log("Prefab found wiith missing script " + path, prefab);
                    break;
                }
            }
        }
    }

    [MenuItem("World's Guadian/Scripts Helper/Find Missing Scripts In Scene")]

    static void FindMissingScriptsInSceneMenuItem()
    {
        foreach (GameObject gameObject in Object.FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None))
        {
            foreach (Component component in gameObject.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    Debug.Log("GameObject found with missing script " + gameObject.name, gameObject);
                    break;
                }
            }
        }
    }
}
