using UnityEngine;

public abstract class AchievementCondition : ScriptableObject
{
    public abstract bool CheckCondition();
}