using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        var unitSpawnerInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }


    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandler = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandler.gameObject);
        }
    }
}
