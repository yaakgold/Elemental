using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    public TMP_InputField charName;
    public TMP_Dropdown charElement;
    public GameObject mainMenuUI;

    public void OnCancel()
    {
        charName.text = "";
        charElement.value = 0;

        mainMenuUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnSave()
    {
        if (charName.text == "") return;

        //SaveSystem.SaveCharacter(charName.text, charElement.itemText.text);
    }
}
