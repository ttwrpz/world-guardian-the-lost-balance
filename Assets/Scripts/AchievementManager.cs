using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    [SerializeField]
    private List<Achievement> achievements;

    private static AchievementManager _instance;

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
        DontDestroyOnLoad(gameObject);

        foreach (Achievement achievement in achievements)
        {
            achievement.Init();
        }
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
