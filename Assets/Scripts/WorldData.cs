using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public string worldName;
    public float completionPercentage;

    public WorldData(string _name, float compPercent = 0)
    {
        worldName = _name;
        completionPercentage = compPercent;
    }
}
