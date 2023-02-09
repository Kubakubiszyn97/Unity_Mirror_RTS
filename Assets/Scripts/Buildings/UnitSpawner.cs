using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform spawnPoint;

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation, null);
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!isOwned) return;

        CmdSpawnUnit();
    }
}
