using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    public bool spawnEnemy = true;
    public GameObject enemyToSpawn;
    public int id;

    private void Start()
    {
        CmdSpawn();
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
