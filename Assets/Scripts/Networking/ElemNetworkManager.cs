using Mirror;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElemNetworkManager : NetworkManager
{
    public static List<GameObject> playerObjs = new List<GameObject>();

    public static GameObject[] spawnPoints;

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        SteamLobby steamLobby = GetComponent<SteamLobby>();

        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        for (int i = 0; i < steamLobby.playerConnections.Count; i++)
        {
            int pChoice = steamLobby.playerElementChoice[steamLobby.playerConnections[i].connectionId];

            GameObject obj = Instantiate(spawnPrefabs[pChoice], spawnPoints[i].transform.position, Quaternion.identity);

            obj.name = steamLobby.players[i].steamName;
            obj.GetComponent<PlayerController>().steamName = obj.name;
            NetworkServer.Spawn(obj, steamLobby.playerConnections[i]);
        }
    }
}
