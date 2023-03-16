using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class AchievementManager : MonoBehaviour
{
    public bool IsLoaded { get; private set; }

    public List<Achievement> achievements;
    private string achievementsSavePath;

    private void Awake()
    {
        achievementsSavePath = Path.Combine(Application.persistentDataPath, "achievements.json");

        LoadAchievementsFromResources();
        LoadSavedAchievements();
    }

    public void CreateSaveFileIfNotExists()
    {
        if (!File.Exists(achievementsSavePath))
        {
            achievements = new();
            SaveAchievements();
        }
    }

    private void LoadAchievementsFromResources()
    {
        achievements.AddRange(Resources.LoadAll<Achievement>("Achievements"));
    }

    private void LoadSavedAchievements()
    {
        if (File.Exists(achievementsSavePath))
        {
            string savedAchievementsJson = File.ReadAllText(achievementsSavePath);
            Achievement[] savedAchievements = JsonUtility.FromJson<Achievement[]>(savedAchievementsJson);

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

        IsLoaded = true;
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