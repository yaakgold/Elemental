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
    private LevelAnimation levelAnimation;

    private void Awake()
    {
        levelText = transform.Find("Level").GetComponent<TMP_Text>();
        experienceBar = transform.Find("ExperienceBar").Find("Fill").GetComponent<Image>();

        transform.Find("ExpButton").GetComponent<Button_UI>().ClickFunc = () => levelSystem.AddExp(500);
    }

    private void SetExpBarSize(float experienceNormalized)
    {
        experienceBar.fillAmount = experienceNormalized;
    }

    private void SetLvlNumber(int lvlNumber)
    {
        levelText.text = "LEVEL " + (lvlNumber + 1);
    }

    public void SetLevel(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;
    }

    public void SetLevelSystemAnimated(LevelAnimation levelAnimation)
    {
        this.levelAnimation = levelAnimation;

        SetLvlNumber(levelAnimation.GetLevelNumber());
        SetExpBarSize(levelAnimation.GetExpNormalized());

        levelAnimation.OnExpChange += LevelAnimation_OnExpChange;
        levelAnimation.OnLvlChange += LevelAnimation_OnLvlChange;
    }

    private void LevelAnimation_OnLvlChange(object sender, System.EventArgs e)
    {
        SetLvlNumber(levelAnimation.GetLevelNumber());
    }

    private void LevelAnimation_OnExpChange(object sender, System.EventArgs e)
    {
        SetExpBarSize(levelAnimation.GetExpNormalized());
    }
}
