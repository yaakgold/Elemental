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
    public GameObject readyToggleBtn;
    public Image background;
    public Button startButton;

    private SteamLobby steamLobby;

    private GameObject player;
    [SerializeField]
    private int spawnChoice;

    [SyncVar]
    private bool readyState = false;

    [SyncVar]
    private ePlayerElement playerElement = ePlayerElement.NONE;

    private int spawnNum = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("PlayersList").transform);

            NetworkManager.singleton.GetComponent<SteamLobby>().players.Add(this);

            //if (!hasAuthority)
            //{
            //    dropdown.value = (int)playerElement;
            //}
        }

    }

    public override void OnStartAuthority()
    {
        //dropdown.interactable = true;

        readyToggleBtn.SetActive(true);

        if(isServer)
        {
            startButton.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        GameObject.FindGameObjectWithTag("LobbyUI").SetActive(false);
        steamLobby.StartGame();
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
    public void OnElementChange(int val)
    {
        if (val == (int)playerElement) return;
        readyToggleBtn.GetComponent<Button>().interactable = true;
        CmdElementChange(val);
    }

    [Command]
    private void CmdElementChange(int newValue)
    {
        if(NetworkManager.singleton.GetComponent<SteamLobby>().playerElementChoice.ContainsKey(connectionToClient.connectionId))
        {
            NetworkManager.singleton.GetComponent<SteamLobby>().playerElementChoice[connectionToClient.connectionId] = newValue;
        }
        else
        {
            NetworkManager.singleton.GetComponent<SteamLobby>().playerElementChoice.Add(connectionToClient.connectionId, newValue);
        }

        RpcElementChange(newValue);
    }

    [ClientRpc]
    private void RpcElementChange(int newValue)
    {
        playerElement = (ePlayerElement)newValue;
    }
    #endregion

    public enum ePlayerElement
    {
        AIR,
        EARTH,
        FIRE,
        WATER,
        NONE
    }
}
