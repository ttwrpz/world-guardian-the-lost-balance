using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill System/Skill", order = 0)]
public class Skill : ScriptableObject
{
    public Sprite icon;
    public string skillName;
    public string description;
    public int skillPointCost;
    public SkillAction action;

    public void ExecuteAction()
    {
        action.PerformAction(this);
    }
}
public class SkillCard
{
    public Skill skill;
    public VisualElement element;

    public SkillCard(Skill skill, VisualElement element)
    {
        this.skill = skill;
        this.element = element;
    }
}
