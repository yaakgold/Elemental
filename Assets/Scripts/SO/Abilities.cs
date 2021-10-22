using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Create Ability")]
public class Abilities : ScriptableObject
{
    public string Description;
    public Sprite sprite;
    public int LvlNeeded;

    public enum FightingStyles
    {
        Air,
        Water,
        Fire,
        Earth
    }

    public FightingStyles fightingStyle = new FightingStyles();
}
