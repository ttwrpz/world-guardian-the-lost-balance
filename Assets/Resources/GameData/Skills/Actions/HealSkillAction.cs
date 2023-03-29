using UnityEngine;

[CreateAssetMenu(fileName = "HealSkillAction", menuName = "Skill System/Actions/Heal")]
public class HealSkillAction : SkillAction
{
    public int healAmount;

    public override void PerformAction(Skill skill)
    {
        // Perform the healing action here
        Debug.Log($"Healing for {healAmount} using {skill.skillName}");
    }
}
