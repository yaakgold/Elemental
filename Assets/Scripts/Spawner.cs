using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    public bool spawnEnemy = true;
    public GameObject enemyToSpawn;
    public int id;

    private void Start()
    {
        GameManager.Instance.spawners.Add(this);

        if (NetworkManager.singleton.GetComponent<SteamLobby>().worldData.spawnObjs.Length > 0)
        {
            spawnEnemy = NetworkManager.singleton.GetComponent<SteamLobby>().worldData.spawnObjs.First(x => x.id == id).spawnEnemy;
        }
        else
            spawnEnemy = true;
    }

    [Command(requiresAuthority = false)]
    public void CmdSpawn()
    {
        if(spawnEnemy)
        {
            NetworkServer.Spawn(Instantiate(enemyToSpawn, transform.position, transform.rotation, transform));
        }
    }
}

[System.Serializable]
public class SpawnObj
{
    public int id;
    public bool spawnEnemy;

    public SpawnObj(int _id, bool _spawnEnemy)
    {
        id = _id;
        spawnEnemy = _spawnEnemy;
    }
}

