using UnityEngine;

public class AchievementControl : MonoBehaviour
{
    public string achievementID;
    private AchievementManager achievementManager;

    private void Start()
    {
        achievementManager = FindFirstObjectByType<AchievementManager>();
    }

    public void CheckAndUnlockAchievement()
    {
        if (achievementManager != null)
        {
            Achievement achievement = achievementManager.GetAchievementByID(achievementID);
            if (achievement != null && !achievement.isUnlocked && achievement.condition.CheckCondition())
            {
                achievementManager.UnlockAchievement(achievementID);
            }
        }
    }
}
