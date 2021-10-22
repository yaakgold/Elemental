using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Create Ability")]
public class Abilities : ScriptableObject
{
    public GameObject ability;

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

    public void DoAbility(Vector3 position, Quaternion rotation)
    {
        Instantiate(ability, position, rotation); 
    }
}
