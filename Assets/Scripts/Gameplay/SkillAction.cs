using UnityEngine;

public abstract class SkillAction : ScriptableObject
{
    public abstract void PerformAction(Skill skill);
}
