using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class LevelAnimation
{
    private LevelSystem lvlSystem;
    private bool isAnimating;
    private int level;
    private int experience;
    private float updateTimer;
    private float updateTimerMax;

    public event EventHandler OnExpChange;
    public event EventHandler OnLvlChange;

    public LevelAnimation(LevelSystem lvlSystem)
    {
        SetLvlSystem(lvlSystem);
        updateTimerMax = .016f;

        FunctionUpdater.Create(() => Update());
    }

    public void SetLvlSystem(LevelSystem lvlSystem)
    {
        this.lvlSystem = lvlSystem;

        level = lvlSystem.GetLevelNumber();
        experience = lvlSystem.GetExp();

        lvlSystem.OnExpChange += LvlSystem_OnExpChange;
        lvlSystem.OnLvlChange += LvlSystem_OnLvlChange;
    }

    private void LvlSystem_OnExpChange(object sender, System.EventArgs e)
    {
        isAnimating = true;
    }

    private void LvlSystem_OnLvlChange(object sender, System.EventArgs e)
    {
        isAnimating = true;
    }

    private void Update()
    {
        if(isAnimating)
        {
            updateTimer += Time.deltaTime;
            while(updateTimer > updateTimerMax)
            {
                updateTimer -= updateTimerMax;
                UpdateAddedExp();
            }
        }
    }

    private void UpdateAddedExp()
    {
        if (level < lvlSystem.GetLevelNumber())
        {
            AddExp();
        }
        else
        {
            if (experience < lvlSystem.GetExp())
            {
                AddExp();
            }
            else
            {
                isAnimating = false;
            }
        }
    }

    private void AddExp()
    {
        experience++;
        if(experience >= lvlSystem.GetExpNextLvl(level))
        {
            level++;
            experience = 0;
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
        if(lvlSystem.IsMaxLvl(level))
        {
            return 1f;
        }
        else
        {
            return experience / (float)lvlSystem.GetExpNextLvl(level);
        }
    }
}
