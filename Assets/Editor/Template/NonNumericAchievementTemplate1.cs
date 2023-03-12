using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonNumericAchievementTemplate : NonNumericAchievement
{
    public override void Init()
    {
        base.Init();
    }

    public override void CheckIfUnlocked()
    {
        base.CheckIfUnlocked();
    }

    public override void Unlock()
    {
        base.Unlock();
        Debug.Log("Achievement Unlocked: " + Name);
    }
}
