using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AchievementContextHandler : MonoBehaviour
{

    [MenuItem("Assets/Create/Achievement/Numeric Achievement Script")]
    public static void CreateNumericAchievementScript()
    {
        string folderPath = GetActiveFolderPath();
        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError("No folder was selected");
            return;
        }

        string scriptName = EditorUtility.SaveFilePanelInProject("Save Numeric Achievement Script", "NewNumericAchievement", "cs", "Enter a file name for the NumericAchievement script", folderPath);
        if (!string.IsNullOrEmpty(scriptName))
        {
            string fileName = Path.GetFileNameWithoutExtension(scriptName);
            string scriptContents = GetScriptContent().Replace("AchievementType", "NumericAchievement").Replace("AchievementName", fileName);

            File.WriteAllText(scriptName, scriptContents);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Assets/Create/Achievement/Non-Numeric Achievement Script")]
    public static void CreateNonNumericAchievementScript()
    {
        string folderPath = GetActiveFolderPath();
        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError("No folder was selected");
            return;
        }

        string scriptName = EditorUtility.SaveFilePanelInProject("Save Non-Numeric Achievement Script", "NewNonNumericAchievement", "cs", "Enter a file name for the NonNumericAchievement script", folderPath);
        if (!string.IsNullOrEmpty(scriptName))
        {
            string fileName = Path.GetFileNameWithoutExtension(scriptName);
            string scriptContents = GetScriptContent().Replace("AchievementType", "NonNumericAchievement").Replace("AchievementName", fileName);

            File.WriteAllText(scriptName, scriptContents);
            AssetDatabase.Refresh();
        }
    }

    private static string GetScriptContent()
    {
        return
@"using UnityEngine;

public class AchievementName : AchievementType
{
    // Implement the methods and properties of the AchievementType class here
}";
    }

    private static string GetActiveFolderPath()
    {
        string folderPath = "";
        UnityEngine.Object obj = Selection.activeObject;
        if (obj != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            if (!string.IsNullOrEmpty(assetPath))
            {
                if (Directory.Exists(assetPath))
                {
                    folderPath = assetPath;
                }
                else
                {
                    folderPath = Path.GetDirectoryName(assetPath);
                }
            }
        }
        return folderPath;
    }
}
