using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourceCost = 10;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    [field: SerializeField] public Targeter Targeter { get; private set; }
    [field: SerializeField] public UnitMovement UnitMovement { get; private set; }
    [SerializeField] private Health healthComponent;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public int ResourceCost => resourceCost;

    public override void OnStartServer()
    {
        healthComponent.ServerOnDie += ServerHandleDie;
        ServerOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        healthComponent.ServerOnDie -= ServerHandleDie;
        ServerOnUnitDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(this.gameObject);
    }

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned || !isClientOnly) { return; }
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!isOwned) return;
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!isOwned) return;
        onDeselected?.Invoke();
    }
}
