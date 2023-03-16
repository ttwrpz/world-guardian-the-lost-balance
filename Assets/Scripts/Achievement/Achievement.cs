using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Achievement")]
public class Achievement : ScriptableObject
{
    public string id;
    public Sprite icon;
    public string achievementName;
    public string description;
    public bool isUnlocked;
    public string unlockedDate;
    public AchievementCondition condition;
}