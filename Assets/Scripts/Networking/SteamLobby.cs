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
    public GameObject worldSelectUI;
    public GameObject joinLobbyUI;

    public List<PlayerSelectUI> players;
    public Dictionary<int, int> playerElementChoice;
    public List<NetworkConnection> playerConnections;

    public GameObject playerUI;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> lobbyJoinReq;
    protected Callback<LobbyEnter_t> lobbyEntered;

    public NetworkManager networkManager;
    private const string HOST_ADDRESS_KEY = "HostAddress";

    public static CSteamID LobbyID { get; private set; }

    private void Start()
    {
        playerElementChoice = new Dictionary<int, int>();
        playerConnections = new List<NetworkConnection>();

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

    public void SelectWorld()
    {
        worldSelectUI.SetActive(true);
        worldSelectUI.GetComponent<WorldCreator>().ShowWorldList();
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

    public void LoadSteamFriendsList()
    {
        joinLobbyUI.SetActive(true);
        for (int i = 0; i < SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll); i++)
        {
            CSteamID steamID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);

            int imageId = SteamFriends.GetLargeFriendAvatar(steamID);

            joinLobbyUI.GetComponent<FriendsList>().AddFriend(SteamFriends.GetFriendPersonaName(steamID), GetSteamImageAsText(imageId), steamID);
        }
    }

    private Texture2D GetSteamImageAsText(int id)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(id, out uint width, out uint height);

        if(isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(id, image, (int)(width * height * 4));

            if(isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
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
                LobbyID,
                HOST_ADDRESS_KEY,
                SteamUser.GetSteamID().ToString());

        lobbyUI.SetActive(true);
    }

    public bool AttemptConnection(CSteamID friendID)
    {
        bool connected = false;

        //SteamFriends.;

        return connected;
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

    public void StartGame()
    {
        print(players.Count);
        foreach (PlayerSelectUI player in players)
        {
            playerConnections.Add(player.connectionToClient);
        }

        NetworkManager.singleton.ServerChangeScene("MainScene");
    }
}
