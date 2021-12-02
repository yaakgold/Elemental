using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public List<Spawner> spawners = new List<Spawner>();
    public WorldData worldData;

    public GameObject playerHealthPanel;
    public GameObject pauseScreen;
    public GameObject exitAndSaveBtn;
    public float completionPercentage = 0;

    private GameObject currentPlayer;

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
        SceneManager.LoadScene(0);
    }

    public void OnSaveGame()
    {
        SpawnObj[] spawnObjs = new SpawnObj[spawners.Count];

        for (int i = 0; i < spawners.Count; i++)
        {
            spawnObjs[i] = new SpawnObj(spawners[i].id, spawners[i].spawnEnemy);
        }

        SaveSystem.SaveWorld("First World", 5, spawnObjs);
    }
}