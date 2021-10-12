using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testing : MonoBehaviour
{
    [SerializeField] private LevelWindow levelWindow;
    [SerializeField] private PlayerController player;

    private void Awake()
    {
        LevelSystem levelSystem = new LevelSystem();
        levelWindow.SetLevel(levelSystem);

        LevelAnimation levelAnimation = new LevelAnimation(levelSystem);
        levelWindow.SetLevelSystemAnimated(levelAnimation);
        player.SetLvlSystemAnim(levelAnimation);
    }
}
