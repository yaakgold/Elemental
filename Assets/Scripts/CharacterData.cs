using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public float[] CharPosition { get; private set; }
    public float[] CharRotation { get; private set; }
    public string CharName { get; private set; }
    public string CharElement { get; private set; }
    public int CharLevelValue { get; private set; }
    public int CharHealth { get; private set; }

    public CharacterData(string name, string element)
    {
        CharName = name;
        CharElement = element;

        CharPosition = new float[3];
        CharRotation = new float[4];
    }
}
