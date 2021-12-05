using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    #region Singleton

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != this && _instance != null)
        {
            Destroy(gameObject);
        }

        _instance = this;
    }

    #endregion

    [SyncVar]
    public List<GameObject> enemies = new List<GameObject>();
    public List<Spawner> spawners = new List<Spawner>();
    public List<GameObject> playerObjs = new List<GameObject>();
    [SyncVar]
    public WorldData worldData;

    public GameObject playerHealthPanel;
    public GameObject pauseScreen;
    public GameObject exitAndSaveBtn;
    [SyncVar]
    public float completionPercentage = 0;

    public TMP_Text enemiesLeftTxt;

    private GameObject currentPlayer;

    private void Start()
    {
        if(isServer)
            CmdSetWorldData();
    }

    private void Update()
    {
        if(!isServer)
        {
            CmdCallUpdate();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdCallUpdate()
    {
        UpdateEnemyUI();
    }

    [Command(requiresAuthority = false)]
    private void CmdSetWorldData()
    {
        worldData = NetworkManager.singleton.GetComponent<SteamLobby>().worldData;
        completionPercentage = worldData.completionPercentage;
        UpdateEnemyUI();
    }

    public void SpawnEnemies()
    {
        foreach (var spawner in spawners)
        {
            spawner.SpawnEnemy();
        }
    }

    public void OnPauseGame(GameObject player)
    {
        currentPlayer = player;
        currentPlayer.GetComponent<StarterAssets.StarterAssetsInputs>().PauseToggle();
    }

    public void OnReturnToGame()
    {
        currentPlayer.GetComponent<StarterAssets.StarterAssetsInputs>().PauseToggle();
    }

    public void OnQuitGame()
    {
        if (isServer)
            NetworkManager.singleton.StopHost();
        else
            NetworkManager.singleton.StopClient();

        //Destroy(NetworkManager.singleton.gameObject);
        Application.Quit();
    }

    public void OnSaveGame()
    {
        SpawnObj[] spawnObjs = new SpawnObj[spawners.Count];

        for (int i = 0; i < spawners.Count; i++)
        {
            spawnObjs[i] = new SpawnObj(spawners[i].id, spawners[i].spawnEnemy);
        }

        PlayerDictionary[] players = new PlayerDictionary[playerObjs.Count];

        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new PlayerDictionary(playerObjs[i].name, playerObjs[i].GetComponent<PlayerController>().GetTotalExp());
        }

        SaveSystem.SaveWorld(worldData.worldName, completionPercentage, spawnObjs, players);
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        if (isServer)
        {
            CmdRemoveEnemy(enemy);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdRemoveEnemy(GameObject enemy)
    {
        RemoveEnemyRpc(enemy);
        completionPercentage = (1 - ((float)enemies.Count / spawners.Count)) * 100;
        UpdateEnemyUI();
    }

    [ClientRpc]
    private void RemoveEnemyRpc(GameObject enemy)
    {
        if(!isServer)
        {
            //enemies.RemoveAt(0);
        }
        enemies.Remove(enemy);
    }

    [Command(requiresAuthority = false)]
    public void CmdAddEnemy(GameObject enemy)
    {
        AddEnemyRpc(enemy);

        completionPercentage = (1 - ((float)enemies.Count / spawners.Count)) * 100;
        UpdateEnemyUI();
    }

    [ClientRpc]
    private void AddEnemyRpc(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    [ClientRpc]
    public void UpdateEnemyUI()
    {
        completionPercentage = (1 - ((float)enemies.Count / spawners.Count)) * 100;
        enemiesLeftTxt.text = $"{completionPercentage}% ENEMIES DEFEATED";
    }
}