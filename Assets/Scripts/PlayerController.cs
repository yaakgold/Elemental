using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private LevelAnimation lvlAnimation;
    public Abilities ability1;
    public Abilities ability2;
    public Abilities ability3;

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

    public void OnAbility1(InputValue input)
    {
        ability1.DoAbility(transform.position, Quaternion.identity);
    }

    public void OnAbility2(InputValue input)
    {
        ability2.DoAbility(transform.position, Quaternion.identity);
    }

    public void OnAbility3(InputValue input)
    {
        ability3.DoAbility(transform.position, Quaternion.identity);
    }
}