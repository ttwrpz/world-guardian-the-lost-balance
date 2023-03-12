using UnityEngine;

public class NonNumericAchievement : Achievement
{
    public override void Unlock()
    {
        _isUnlocked = true;
        Debug.Log("Achievement Unlocked: " + Name);
    }

    public override void CheckIfUnlocked() { }
}
