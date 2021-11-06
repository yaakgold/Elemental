using Mirror;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElemNetworkManager : NetworkManager
{
    [Client]
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        print(conn.connectionId);
        GameObject player = Instantiate(spawnPrefabs.Find(x => x.name.Contains("AirBender")), new Vector3(0, 100, 0), Quaternion.identity);
        NetworkServer.Spawn(player, conn);
    }
}
