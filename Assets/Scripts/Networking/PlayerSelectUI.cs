using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerSelectUI : NetworkBehaviour
{
    public TMP_Text userName_txt;
    public TMP_Dropdown dropdown;
    public GameObject readyToggleBtn;
    public Image background;
    public Button startButton;

    private SteamLobby steamLobby;

    [SyncVar]
    private bool readyState = false;

    [SyncVar]
    private ePlayerElement playerElement = ePlayerElement.NONE;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
            return;

        transform.SetParent(GameObject.FindGameObjectWithTag("PlayersList").transform);

        NetworkManager.singleton.GetComponent<SteamLobby>().players.Add(this);

        if(!hasAuthority)
        {
            dropdown.value = (int)playerElement;
        }
    }

    public override void OnStartAuthority()
    {
        dropdown.interactable = true;

        readyToggleBtn.SetActive(true);

        if(isServer)
        {
            startButton.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        GameObject.FindGameObjectWithTag("LobbyUI").SetActive(false);

        NetworkManager.singleton.ServerChangeScene("MainScene");
    }

    #region Toggle Ready State
    public void ToggleReadyState()
    {
        CmdToggleReady();
    }

    [Command]
    public void CmdToggleReady()
    {
        RpcToggleReady();
    }

    [ClientRpc]
    public void RpcToggleReady()
    {
        readyState = !readyState;

        background.color = readyState ? Color.green : Color.red;

        if (!isServer) return;

        if (steamLobby == null)
            steamLobby = NetworkManager.singleton.GetComponent<SteamLobby>();

        foreach (PlayerSelectUI player in steamLobby.players)
        {
            if (!player.readyState) return;
        }

        startButton.interactable = true;
    }
    #endregion

    #region Choose Element
    public void OnElementChange()
    {
        if (dropdown.value == (int)playerElement) return;

        CmdElementChange(dropdown.value);
    }

    [Command]
    private void CmdElementChange(int newValue)
    {
        //TODO: Check here to make sure that nobody else is using the currently selected element
        //TODO: Change the way the player chooses their element so the server can verify if that element is available
        //if(steamLobby == null)
        //    steamLobby = NetworkManager.singleton.GetComponent<SteamLobby>();

        //foreach (PlayerSelectUI player in steamLobby.players)
        //{
        //    if (player == this)
        //        continue;

        //    if (player.playerElement == playerElement)
        //        return;
        //}

        RpcElementChange(newValue);
    }

    [ClientRpc]
    private void RpcElementChange(int newValue)
    {
        playerElement = (ePlayerElement)newValue;

        dropdown.value = newValue;
    }
    #endregion

    public enum ePlayerElement
    {
        NONE,
        AIR,
        EARTH,
        FIRE,
        WATER
    }
}
