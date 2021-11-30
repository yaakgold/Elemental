using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystemSetup : MonoBehaviour
{
    [SerializeField] private LevelWindow levelWindow;
    [SerializeField] private PlayerController player;

    private void Start()
    {
        //Set levelWindow and player
        levelWindow = GameObject.FindGameObjectWithTag("LevelWindow")?.GetComponent<LevelWindow>();
        player = GetComponent<PlayerController>();

        LevelSystem levelSystem = new LevelSystem();
        levelWindow.SetLevel(levelSystem);

        LevelAnimation levelAnimation = new LevelAnimation(levelSystem);
        levelWindow.SetLevelSystemAnimated(levelAnimation);
        player.SetLvlSystemAnim(levelAnimation);
    }
}
