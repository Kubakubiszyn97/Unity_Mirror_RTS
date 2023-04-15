using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;
    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    [SerializeField] GameObject buildingPreview;
    [SerializeField] private Sprite icon;
    [SerializeField] private int id = -1;
    [SerializeField] private int price = 100;

    public Sprite Icon => icon;
    public int Price => price;
    public int Id => id;
    public GameObject BuildingPreview => buildingPreview;

    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }

    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isOwned || !isClientOnly) { return; }
        AuthorityOnBuildingDespawned?.Invoke(this);
    }

}
