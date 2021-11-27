using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject charCreator;
    public GameObject startGameUI;
    public GameObject controlsUI;
    public GameObject creditsUI;
    public GameObject returnBtn;

    private GameObject currentActivePanel = null;
    public void OnCreateCharClick()
    {
        mainMenu.SetActive(false);
        charCreator.SetActive(true);
    }

    public void OnStartGameUI()
    {
        mainMenu.SetActive(false);
        startGameUI.SetActive(true);
        currentActivePanel = startGameUI;
        returnBtn.SetActive(true);
    }

    public void OnControlsPanelUI()
    {
        mainMenu.SetActive(false);
        controlsUI.SetActive(true);
        currentActivePanel = controlsUI;
        returnBtn.SetActive(true);
    }

    public void OnCreditsPanelUI()
    {
        mainMenu.SetActive(false);
        creditsUI.SetActive(true);
        currentActivePanel = creditsUI;
        returnBtn.SetActive(true);
    }

    public void BackToMainMenuScreen()
    {
        mainMenu.SetActive(true);
        currentActivePanel.SetActive(false);
        currentActivePanel = null;
        returnBtn.SetActive(false);
    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}
