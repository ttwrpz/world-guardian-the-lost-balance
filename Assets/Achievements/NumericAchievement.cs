using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievements/Numeric Achievement/New")]
public class NumericAchievement : Achievement
{
    [SerializeField]
    private int _goal;

    public int Goal
    {
        get { return _goal; }
        set { _goal = value; }
    }

    [SerializeField]
    private int _progress;

    public int Progress
    {
        get { return _progress; }
        set { _progress = value; }
    }

    public override void Unlock()
    {
        IsUnlocked = true;
        Debug.Log("Achievement Unlocked: " + Name);
    }

    public override void CheckIfUnlocked()
    {
        if (_progress >= _goal)
        {
            Unlock();
        }
    }

    public void SetProgress(int progress)
    {
        _progress = progress;
        CheckIfUnlocked();
    }
}
