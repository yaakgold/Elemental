using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private LevelAnimation lvlAnimation;
    public Abilities ability1;
    public Abilities ability2;
    public GameObject ability1UI;
    public GameObject ability2UI;

    [SerializeField]
    private Transform camFollow;

    private void Start()
    {
        camFollow = GameObject.FindGameObjectWithTag("CinemachineTarget").transform;
        GameObject.FindGameObjectWithTag("CinemachineCam").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = camFollow;

        //Setup Abilities
        ability1UI = GameObject.FindGameObjectsWithTag("AbilityUI")[0];
        ability2UI = GameObject.FindGameObjectsWithTag("AbilityUI")[1];

        ability1UI.GetComponentInChildren<TMP_Text>().text = ability1.name;
        ability1UI.GetComponentInChildren<Image>().sprite = ability1.sprite;

        ability2UI.GetComponentInChildren<TMP_Text>().text = ability2.name;
        ability2UI.GetComponentInChildren<Image>().sprite = ability2.sprite;
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