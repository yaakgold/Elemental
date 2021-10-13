using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem
{
    private int level;
    private int experience;
    private int experienceNextLevel;

    public event EventHandler OnExpChange;
    public event EventHandler OnLvlChange;


    public LevelSystem()
    {
        level = 0;
        experience = 0;
        experienceNextLevel = 100;
    }

    public void AddExp(int xp)
    {
        experience += xp;
        if(experience >= experienceNextLevel)
        {
            level++;
            experience -= experienceNextLevel;
            if (OnLvlChange != null) OnLvlChange(this, EventArgs.Empty);
        }
        if (OnExpChange != null) OnExpChange(this, EventArgs.Empty);
    }

    public int GetLevelNumber()
    {
        return level;
    }

    public float GetExpNormalized()
    {
        return experience / (float)experienceNextLevel;
    }
}
