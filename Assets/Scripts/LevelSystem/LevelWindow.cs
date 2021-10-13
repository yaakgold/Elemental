using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeMonkey.Utils;

public class LevelWindow : MonoBehaviour
{
    private TMP_Text levelText;
    private Button expButton;
    private Image experienceBar;
    private LevelSystem levelSystem;

    private void Awake()
    {
        levelText = transform.Find("levelText").GetComponent<TMP_Text>();
        experienceBar = transform.Find("experienceBar").Find("bar").GetComponent<Image>();

        transform.Find("ExpButton").GetComponent<Button_UI>().ClickFunc = () => levelSystem.AddExp(5);
    }

    private void SetExpBarSize(float experienceNormalized)
    {
        experienceBar.fillAmount = experienceNormalized;
    }

    private void SetLvlNumber(int lvlNumber)
    {
        levelText.text = "LEVEL " + (lvlNumber + 1);
    }

    private void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;

        SetLvlNumber(levelSystem.GetLevelNumber());
        SetExpBarSize(levelSystem.GetExpNormalized());

        levelSystem.OnExpChange += LevelSystem_OnExpChange;
        levelSystem.OnLvlChange += LevelSystem_OnLvlChange;
    }

    private void LevelSystem_OnLvlChange(object sender, System.EventArgs e)
    {
        SetLvlNumber(levelSystem.GetLevelNumber());
    }

    private void LevelSystem_OnExpChange(object sender, System.EventArgs e)
    {
        SetExpBarSize(levelSystem.GetExpNormalized());
    }
}
