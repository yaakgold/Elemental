using Mirror;
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

    [SerializeField]
    private Animator anim;

    public void OnCreateCharClick()
    {
        mainMenu.SetActive(false);
        charCreator.SetActive(true);
    }

    public void OnStartGameUI()
    {
        mainMenu.SetActive(false);
        //startGameUI.SetActive(true);
        NetworkManager.singleton.GetComponent<SteamLobby>().SelectWorld();
        currentActivePanel = NetworkManager.singleton.GetComponent<SteamLobby>().worldSelectUI;
        returnBtn.SetActive(true);
        anim.SetTrigger("Fire");
    }

    public void OnControlsPanelUI()
    {
        mainMenu.SetActive(false);
        controlsUI.SetActive(true);
        currentActivePanel = controlsUI;
        returnBtn.SetActive(true);
        anim.SetTrigger("Earth");
    }

    public void OnCreditsPanelUI()
    {
        mainMenu.SetActive(false);
        creditsUI.SetActive(true);
        currentActivePanel = creditsUI;
        returnBtn.SetActive(true);
        anim.SetTrigger("Ice");
    }

    public void BackToMainMenuScreen()
    {
        if (!currentActivePanel) return;
        mainMenu.SetActive(true);
        currentActivePanel.SetActive(false);
        currentActivePanel = null;
        returnBtn.SetActive(false);
        anim.SetTrigger("Return");
    }

    public void OnMasterChange(float value)
    {
        foreach (var sound in AudioManager.Instance.sounds)
        {
            sound.vol = value / 100;
            sound.Update();
        }
    }

    public void OnSFXChange(float value)
    {
        foreach (var sound in AudioManager.Instance.sounds)
        {
            if(sound.isSFX)
            {
                sound.vol = value / 100;
                sound.Update();
            }
        }
    }

    public void OnMusicChange(float value)
    {
        foreach (var sound in AudioManager.Instance.sounds)
        {
            if(!sound.isSFX)
            {
                sound.vol = value / 100;
                sound.Update();
            }
        }
    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}
