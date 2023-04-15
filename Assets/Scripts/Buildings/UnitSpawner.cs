using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text remainingUnitText;
    [SerializeField] private Image unitProgresImage;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private Health healthComponent;
    [SerializeField] private Transform spawnPoint;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private RTSPlayer player;
    private float progressFillVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (queuedUnits == maxUnitQueue) return;
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        if (player.Resources < unitPrefab.ResourceCost) return;

        queuedUnits++;

        player.SetResources(player.Resources - unitPrefab.ResourceCost);
    }

    public override void OnStartServer()
    {
        healthComponent.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        healthComponent.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0) return;

        unitTimer += Time.deltaTime;

        if (unitTimer < unitSpawnDuration) return;

        var unitInstance = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation, null);
        NetworkServer.Spawn(unitInstance.gameObject, connectionToClient);

        Vector3 spawnOffset = UnityEngine.Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = spawnPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(spawnPoint.position + spawnOffset);

        queuedUnits--;
        unitTimer = 0f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!isOwned) return;

        CmdSpawnUnit();
    }

    [Client]
    private void ClientHandleQueuedUnitsUpdated(int oldValue, int newValue)
    {
        remainingUnitText.text = newValue.ToString();
    }

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnDuration;

        if (newProgress < unitProgresImage.fillAmount)
        {
            unitProgresImage.fillAmount = newProgress;
        }
        else
        {
            unitProgresImage.fillAmount = Mathf.SmoothDamp(unitProgresImage.fillAmount, newProgress, ref progressFillVelocity, .1f);
        }
    }
}
