using System;
using UnityEngine;

public abstract class Achievement : ScriptableObject
{
    [SerializeField]
    private protected string _id;

    public string Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [SerializeField]
    private protected Sprite _icon;

    public Sprite Icon
    {
        get { return _icon; }
        set { _icon = value; }
    }

    [SerializeField]
    private protected string _name;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    [SerializeField]
    private protected string _description;

    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }


    [SerializeField]
    private protected bool _isUnlocked;

    public bool IsUnlocked
    {
        get { return _isUnlocked; }
        set { _isUnlocked = value; }
    }

    private DateTime _unlockedDate;

    public DateTime UnlockedDate
    {
        get { return _unlockedDate; }
        set { _unlockedDate = value; }
    }


    public virtual void Init()
    {
        _isUnlocked = false;
        CheckIfUnlocked();
    }

    public abstract void Unlock();

    public virtual void CheckIfUnlocked() { }
}