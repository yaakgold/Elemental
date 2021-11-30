using Mirror;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElemNetworkManager : NetworkManager
{
    public static List<GameObject> playerObjs = new List<GameObject>();

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        SteamLobby steamLobby = GetComponent<SteamLobby>();

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        for (int i = 0; i < steamLobby.playerConnections.Count; i++)
        {
            int pChoice = steamLobby.playerElementChoice[steamLobby.playerConnections[i].connectionId];

            GameObject obj = Instantiate(spawnPrefabs[pChoice], spawnPoints[i].transform.position, Quaternion.identity);

            print(steamLobby.playerConnections[i]);
            NetworkServer.Spawn(obj, steamLobby.playerConnections[i]);
        }
    }
}
