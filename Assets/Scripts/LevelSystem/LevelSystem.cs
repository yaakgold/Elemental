using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem
{
    private int level;
    private int experience;

    private static readonly int[] expPerLvl = new[] { 100, 120, 140, 160, 180, 200, 240, 280, 320, 380, 440, 500, 600, 700, 800, 920, 1040, 1160, 1300, 1440, 1580, 1740, 1900, 2060, 2240, 2420, 2600, 2800, 3000 };

    public event EventHandler OnExpChange;
    public event EventHandler OnLvlChange;


    public LevelSystem()
    {
        level = 0;
        experience = 0;
    }

    public void AddExp(int xp)
    {
        if(!IsMaxLvl())
        {
            experience += xp;
            while(!IsMaxLvl() && experience >= GetExpNextLvl(level))
            {
                experience -= GetExpNextLvl(level);
                level++;
                if (OnLvlChange != null) OnLvlChange(this, EventArgs.Empty);
            }
            if (OnExpChange != null) OnExpChange(this, EventArgs.Empty);
        }
    }

    public int GetLevelNumber()
    {
        return level;
    }

    public float GetExpNormalized()
    {
        if(IsMaxLvl())
        {
            return 1f;
        }
        else
        {
            return experience / (float)GetExpNextLvl(level);
        }

    }

    public int GetExp()
    {
        return experience;
    }

    public int GetExpNextLvl(int level)
    {
        if(level < expPerLvl.Length)
        {
            return expPerLvl[level];
        }
        else
        {
            Debug.Log("Level Invalid: " + level);
            return 100;
        }
    }

    public bool IsMaxLvl()
    {
        return IsMaxLvl(level);
    }

    public bool IsMaxLvl(int level)
    {
        return level == expPerLvl.Length - 1;
    }
}
