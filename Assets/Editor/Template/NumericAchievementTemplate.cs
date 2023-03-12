using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumericAchievementTemplate : NumericAchievement
{
    public override void Init()
    {
        base.Init();
        Goal = Goal; // Need to be replace
    }

    public override void CheckIfUnlocked()
    {
        base.CheckIfUnlocked();

        if (Progress >= Goal)
        {
            Unlock();
        }
    }

    public override void Unlock()
    {
        base.Unlock();
        Debug.Log("Achievement Unlocked: " + Name);
    }
}
