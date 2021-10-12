using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private LevelAnimation lvlAnimation;


    public void SetLvlSystemAnim(LevelAnimation lvlAnimation)
    {
        this.lvlAnimation = lvlAnimation;

        lvlAnimation.OnLvlChange += LvlSystem_OnLvlChange;
    }

    private void LvlSystem_OnLvlChange(object sender, System.EventArgs e)
    {
        //Implement level up ding or particle effect.
        print("Level Up");
    }
}