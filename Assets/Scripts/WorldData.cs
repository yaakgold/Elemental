using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public string worldName;
    public float completionPercentage;
    public SpawnObj[] spawnObjs;
    public PlayerDictionary[] players;

    public WorldData()
    {

    }

    public WorldData(string _name, float compPercent, SpawnObj[] _spawnObjs, PlayerDictionary[] _players)
    {
        worldName = _name;
        completionPercentage = compPercent;
        spawnObjs = _spawnObjs;
        players = _players;
    }
}

[System.Serializable]
public class PlayerDictionary
{
    public string playerName;
    public int playerExp;

    public PlayerDictionary()
    {
        
    }

    public PlayerDictionary(string name, int exp)
    {
        playerName = name;
        playerExp = exp;
    }
}