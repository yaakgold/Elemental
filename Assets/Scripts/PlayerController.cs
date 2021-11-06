using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private LevelAnimation lvlAnimation;
    public Abilities ability1;
    public Abilities ability2;

    [SerializeField]
    private Transform camFollow;

    private void Start()
    {
        camFollow = GameObject.FindGameObjectWithTag("CinemachineTarget").transform;
        GameObject.FindGameObjectWithTag("CinemachineCam").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = camFollow;
    }

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