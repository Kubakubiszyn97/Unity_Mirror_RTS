using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdated;

    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    public override void OnStartServer()
    {
        UnitBase.ServerOnPlayerDie += HandlePlayerDie;
        currentHealth = maxHealth;
    }


    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= HandlePlayerDie;
    }

    [Server]
    private void HandlePlayerDie(int connectionId)
    {
        if (connectionToClient.connectionId != connectionId) return;

        DealDamage(currentHealth);
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (currentHealth == 0) return;

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth != 0) return;
        ServerOnDie?.Invoke();
        Debug.Log("Die");
    }

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }
}
