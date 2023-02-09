using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(unitSpawnerPrefab, conn);
    }
}
