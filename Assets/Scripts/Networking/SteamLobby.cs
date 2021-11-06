using kcp2k;
using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
    public bool useKCP = false;

    public GameObject mainMenu;
    public GameObject lobbyUI;

    public List<PlayerSelectUI> players;
    public List<int> playerElementChoice;

    public GameObject playerUI;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> lobbyJoinReq;
    protected Callback<LobbyEnter_t> lobbyEntered;

    public NetworkManager networkManager;
    private const string HOST_ADDRESS_KEY = "HostAddress";

    public static CSteamID LobbyID { get; private set; }

    private void Start()
    {
        if(networkManager == null)
        {
            return;
        }

        if (!SteamManager.Initialized)
        {
            return;
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyJoinReq = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        players = new List<PlayerSelectUI>();

        if (!networkManager)
        {
            Debug.LogError("Net Man not set");

            return;
        }

        mainMenu.SetActive(false);

        if(!useKCP)
        {
            SteamMatchmaking.CreateLobby(
                    ELobbyType.k_ELobbyTypeFriendsOnly,
                    networkManager.maxConnections);
        }
        else
        {
            lobbyUI.SetActive(true);
            networkManager.StartHost();
        }
    }

    public void JoinLobby()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();

        mainMenu.SetActive(false);
        lobbyUI.SetActive(true);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            mainMenu.SetActive(true); 
            lobbyUI.SetActive(false);
            return;
        }

        LobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HOST_ADDRESS_KEY,
                SteamUser.GetSteamID().ToString());

        lobbyUI.SetActive(true);
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return;

        string hostAddress = SteamMatchmaking.GetLobbyData(
                                new CSteamID(callback.m_ulSteamIDLobby),
                                HOST_ADDRESS_KEY);

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        mainMenu.SetActive(false);
        lobbyUI.SetActive(true);
    }
}
