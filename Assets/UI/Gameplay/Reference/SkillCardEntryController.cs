using UnityEngine;
using UnityEngine.UIElements;

public class SkillCardEntryController
{
    GroupBox SkillCardWrapper;
    VisualElement Icon;
    VisualElement SkillInfoIcon;
    Label NameLabel;
    Label SkillPointUsed;

    public void SetVisualElement(VisualElement visualElement)
    {
        SkillCardWrapper = visualElement.Q<GroupBox>("SkillCardWrapper");
        Icon = visualElement.Q<VisualElement>("Icon");
        SkillInfoIcon = visualElement.Q<VisualElement>("SkillInfoIcon");
        NameLabel = visualElement.Q<Label>("NameLabel");
        SkillPointUsed = visualElement.Q<Label>("SkillPointUsed");
    }

    public void SetData(Skill skill)
    {
        if (skill.icon != null)
        {
            Icon.style.backgroundImage = new StyleBackground(skill.icon);
        }
        else
        {
            Icon.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>("GUI/question"));
        }

        NameLabel.text = skill.skillName;
        SkillPointUsed.text = skill.skillPointCost.ToString();
    }

    public VisualElement GetSkillInfoIcon()
    {
        return SkillInfoIcon;
    }
}
