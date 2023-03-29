using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public List<Achievement> achievements;
    private string achievementsSavePath;

    private void Awake()
    {
        achievementsSavePath = Path.Combine(Application.persistentDataPath, "achievements.json");
        CreateSaveFileIfNotExists();
        LoadSavedAchievements();
        LoadAchievementsFromResources();
    }

    public void CreateSaveFileIfNotExists()
    {
        if (!File.Exists(achievementsSavePath))
        {
            achievements = new();
            SaveAchievements();
        }
    }

    public void LoadAchievementsFromResources()
    {
        Achievement[] resourcesAchievements = Resources.LoadAll<Achievement>("GameData/Achievements");

        if (resourcesAchievements.Length > 0)
        {
            foreach (Achievement resourceAchievement in resourcesAchievements)
            {
                if (!achievements.Exists(a => a.id == resourceAchievement.id))
                {
                    achievements.Add(resourceAchievement);
                }
            }
        }
    }

    public void LoadSavedAchievements()
    {
        if (File.Exists(achievementsSavePath))
        {
            string savedAchievementsJson = File.ReadAllText(achievementsSavePath);
            Achievement[] savedAchievements = JsonUtility.FromJson<Achievement[]>(savedAchievementsJson);

            if (savedAchievements.Length > 0)
            {
                foreach (Achievement savedAchievement in savedAchievements)
                {
                    Achievement achievement = GetAchievementByID(savedAchievement.id);
                    if (achievement != null)
                    {
                        achievement.isUnlocked = savedAchievement.isUnlocked;
                        achievement.unlockedDate = savedAchievement.unlockedDate;
                    }
                }
            }
        }
    }

    public void SaveAchievements()
    {
        string achievementsJson = JsonUtility.ToJson(achievements.ToArray(), true);
        File.WriteAllText(achievementsSavePath, achievementsJson);
    }

    public List<Achievement> GetAchievements()
    {
        return achievements;
    }

    public List<Achievement> GetUnlockedAchievements()
    {
        if (achievements == null || achievements.Count == 0)
        {
            Debug.LogWarning("No achievements found. Make sure they are loaded correctly.");
            return new List<Achievement>();
        }
        return achievements.Where(a => a.isUnlocked).ToList();
    }

    public Achievement GetAchievementByID(string id)
    {
        return achievements.Find(achievement => achievement.id == id);
    }

    public void UnlockAchievement(string id)
    {
        Achievement achievement = GetAchievementByID(id);
        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.isUnlocked = true;
            achievement.unlockedDate = System.DateTime.Now.ToString();
            SaveAchievements();
        }
    }
}