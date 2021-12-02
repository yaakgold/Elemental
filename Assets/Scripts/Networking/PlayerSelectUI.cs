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
    public Button[] buttons;

    [SyncVar(hook = "SetName")]
    public string steamName;

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
        }

        if (hasAuthority)
        {
            steamLobby = NetworkManager.singleton.GetComponent<SteamLobby>();
            CmdChangeSteamName(steamLobby.GetSteamName());
        }
    }

    [Command]
    private void CmdChangeSteamName(string newName)
    {
        steamName = newName;
    }

    private void SetName(string oldName, string newName)
    {
        userName_txt.text = newName;
    }

    public override void OnStartAuthority()
    {
        //dropdown.interactable = true;

        readyToggleBtn.SetActive(true);

        if(isServer)
        {
            startButton.gameObject.SetActive(true);
        }

        foreach (var button in buttons)
        {
            button.interactable = true;
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

        readyToggleBtn.GetComponent<Image>().color = readyState ? Color.green : Color.white;

        if (!isServer) return;

        if (steamLobby == null)
            steamLobby = NetworkManager.singleton.GetComponent<SteamLobby>();

        startButton.interactable = false;

        if(isLocalPlayer)
        {
            foreach (var button in buttons)
            {
                button.interactable = !readyState;
            }
        }

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

        foreach (var button in buttons)
        {
            button.GetComponent<Image>().color = Color.white;
        }

        buttons[newValue].GetComponent<Image>().color = buttons[newValue].colors.highlightedColor;

        switch (playerElement)
        {
            case ePlayerElement.AIR:
                userName_txt.color = Color.yellow;
                break;
            case ePlayerElement.EARTH:
                userName_txt.color = Color.green;
                break;
            case ePlayerElement.FIRE:
                userName_txt.color = Color.red;
                break;
            case ePlayerElement.WATER:
                userName_txt.color = Color.cyan;
                break;
            default:
                break;
        }
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
