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
        CmdUpdateEnemyList();
        if (!isServer)
        {
            CmdCallUpdate();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateEnemyList()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i] == null)
            {
                enemies.RemoveAt(i);
            }
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

    public void RemoveEnemyFromList(NetworkIdentity netIDent)
    {
        print(isServer);
        if (isServer)
        {
            CmdRemoveEnemy(netIDent);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdRemoveEnemy(NetworkIdentity netIdent)
    {
        GameObject go = enemies.Find(x => x.GetComponent<NetworkIdentity>().netId == netIdent.netId);

        EnemyAI ai = go.GetComponent<EnemyAI>();
        if (ai.lastBlow == null) return;
        if (ai.lastBlow.TryGetComponent(out PlayerController pc))
        {
            pc.AddExp(ai.expAmount);
        }
        completionPercentage = (1 - ((float)enemies.Count / spawners.Count)) * 100;
        UpdateEnemyUI();
    }

    [Command(requiresAuthority = false)]
    public void CmdAddEnemy(NetworkIdentity netIdent)
    {
        AddEnemyRpc(netIdent);

        completionPercentage = (1 - ((float)enemies.Count / spawners.Count)) * 100;
        UpdateEnemyUI();
    }

    [ClientRpc]
    private void AddEnemyRpc(NetworkIdentity netIddent)
    {
        GameObject go = GameObject.FindGameObjectsWithTag("Enemy").FirstOrDefault(x =>
        {
            if (x.TryGetComponent(out NetworkIdentity ident))
            {
                return ident.netId == netIddent.netId;
            }
            return false;
        });

        if(go)
        {
            enemies.Add(go);
        }
    }

    [ClientRpc]
    public void UpdateEnemyUI()
    {
        completionPercentage = (1 - ((float)enemies.Count / spawners.Count)) * 100;
        enemiesLeftTxt.text = $"{completionPercentage}% ENEMIES DEFEATED";
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
            if (sound.isSFX)
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
            if (!sound.isSFX)
            {
                sound.vol = value / 100;
                sound.Update();
            }
        }
    }
}