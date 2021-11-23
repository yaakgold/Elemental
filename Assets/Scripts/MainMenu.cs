using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject charCreator;
    public GameObject startGameUI;

    public void OnCreateCharClick()
    {
        mainMenu.SetActive(false);
        charCreator.SetActive(true);
    }

    public void OnStartGameUI()
    {
        mainMenu.SetActive(false);
        startGameUI.SetActive(true);
    }
}
