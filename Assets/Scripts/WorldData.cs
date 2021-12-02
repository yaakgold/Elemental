using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public string worldName;
    public float completionPercentage;
    public SpawnObj[] spawnObjs;

    public WorldData()
    {

    }

    public WorldData(string _name, float compPercent, SpawnObj[] _spawnObjs)
    {
        worldName = _name;
        completionPercentage = compPercent;
        spawnObjs = _spawnObjs;
    }
}
