using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    public int maxSkillPoints = 100;
    public int playerSkillPoints = 0;

    public List<Skill> skills = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public List<Skill> LoadSkills()
    {
        skills.Clear();

        Skill[] loadedSkills = Resources.LoadAll<Skill>("GameData/Skills");

        foreach (Skill skill in loadedSkills)
        {
            skills.Add(skill);
        }

        return skills;
    }

    public bool CanUseSkill(Skill skill)
    {
        return playerSkillPoints >= skill.skillPointCost;
    }

    public void UseSkill(Skill skill)
    {
        if (CanUseSkill(skill))
        {
            playerSkillPoints -= skill.skillPointCost;
            skill.ExecuteAction();
        }
        else
        {
            Debug.LogWarning("Not enough skill points to use this skill.");
        }
    }

    public void AddSkillPoints(int amount, World.Difficulty difficulty)
    {
        float multiplier = 1f;

        switch (difficulty)
        {
            case World.Difficulty.Easy:
                multiplier = 1f;
                break;
            case World.Difficulty.Medium:
                multiplier = 0.5f;
                break;
            case World.Difficulty.Hard:
                multiplier = 0.25f;
                break;
        }

        playerSkillPoints = (int)Mathf.Clamp(playerSkillPoints + (amount * multiplier), 0, maxSkillPoints);
    }

}