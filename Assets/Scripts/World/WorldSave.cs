using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldSave
{
    public float ElapsedTime { get; set; }
    public List<City> Cities { get; set; }

    public int SkillPoints { get; set; }
}
