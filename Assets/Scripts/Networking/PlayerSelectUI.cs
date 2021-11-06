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

    private GameObject player;
    [SerializeField]
    private int spawnChoice;

    [SyncVar]
    private bool readyState = false;

    [SyncVar]
    private ePlayerElement playerElement = ePlayerElement.NONE;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (!hasAuthority) return;

            SpawnPlayer();
        }
        else
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("PlayersList").transform);

            NetworkManager.singleton.GetComponent<SteamLobby>().players.Add(this);

            if(!hasAuthority)
            {
                dropdown.value = (int)playerElement;
            }
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

    #region Spawn Player
    private void SpawnPlayer()
    {
        Vector3 spawnPos = GameObject.FindGameObjectsWithTag("SpawnPoint")[connectionToClient.connectionId].transform.position;

        spawnChoice = NetworkManager.singleton.GetComponent<SteamLobby>().playerElementChoice[connectionToClient.connectionId];

        print(spawnChoice);
        player = Instantiate(NetworkManager.singleton.spawnPrefabs[spawnChoice], spawnPos, Quaternion.identity);

        CmdSpawnPlayer(connectionToClient.identity.connectionToClient);
    }

    [Command]
    private void CmdSpawnPlayer(NetworkConnectionToClient conn = null)
    {
        NetworkServer.Spawn(player, conn);
    }
    #endregion

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
        NetworkManager.singleton.GetComponent<SteamLobby>().playerElementChoice[connectionToClient.connectionId] = dropdown.value - 1;
        CmdElementChange(dropdown.value);
    }

    [Command]
    private void CmdElementChange(int newValue)
    {
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
