using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementManager : MonoBehaviour
{
    private static AchievementManager _instance;

    [SerializeField]
    private List<Achievement> achievements;

    public static AchievementManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<AchievementManager>();

                if (_instance == null)
                {
                    GameObject go = new()
                    {
                        name = "AchievementManager"
                    };
                    _instance = go.AddComponent<AchievementManager>();
                }
            }

            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (Achievement achievement in achievements)
        {
            achievement.Init();
        }
    }

    public static List<Achievement> GetAchievements()
    {
        AchievementManager manager = FindAnyObjectByType<AchievementManager>();
        if (manager != null)
        {
            return manager.achievements;
        }
        else
        {
            Debug.LogWarning("Could not find AchievementManager in the scene! Trying to create one.");

            GameObject go = new("AchievementManager");
            manager = go.GetComponent<AchievementManager>();

            return manager.achievements;
        }
    }

    public static List<Achievement> GetUnlockedAchievements()
    {
        AchievementManager manager = FindAnyObjectByType<AchievementManager>();
        if (manager != null)
        {
            List<Achievement> unlockedAchievements = new List<Achievement>();
            foreach (Achievement achievement in manager.achievements)
            {
                if (achievement.IsUnlocked)
                {
                    unlockedAchievements.Add(achievement);
                }
            }
            return unlockedAchievements;
        }
        else
        {
            Debug.LogWarning("Could not find AchievementManager in the scene! Trying to create one.");

            GameObject go = new("AchievementManager");
            manager = go.GetComponent<AchievementManager>();

            return manager.achievements;
        }
    }

    public static void LoadAchievementsFromResourcesFolder()
    {
        AchievementManager manager = Instance;
        if (manager == null)
        {
            GameObject go = new("AchievementManager");
            manager = go.AddComponent<AchievementManager>();
        }

        Achievement[] achievements = Resources.LoadAll<Achievement>("Achievements");

        foreach (Achievement achievement in achievements)
        {
            if (!manager.achievements.Contains(achievement))
            {
                manager.achievements.Add(achievement);
            }
        }
    }

    public static void AddAchievement(Achievement achievement)
    {
        AchievementManager manager = Instance;

        if (manager != null)
        {
            manager.achievements.Add(achievement);
        }
        else
        {
            Debug.LogWarning("Could not find AchievementManager in the scene! Trying to create one.");

            GameObject go = new("AchievementManager");
            manager = go.AddComponent<AchievementManager>();
            manager.achievements = new List<Achievement>
            {
                achievement
            };
        }
    }

    public static bool RemoveAchievement(string id)
    {
        AchievementManager manager = FindAnyObjectByType<AchievementManager>();
        if (manager != null)
        {
            Achievement achievement = manager.achievements.Find(a => a.Id == id);

            if (achievement != null)
            {
                manager.achievements.Remove(achievement);
                return true;
            }
        }
        else
        {
            Debug.LogWarning("Could not find AchievementManager in the scene!");
        }

        return false;
    }

    public void UnlockAchievement(string id)
    {
        Achievement achievement = achievements.Find(a => a.Id == id);

        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.Unlock();
        }
    }
}
