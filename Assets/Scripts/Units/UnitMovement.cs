using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;

    private Camera mainCamera;
    
    [Command]
    private void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out var hit, 1f, NavMesh.AllAreas)) return;

        agent.SetDestination(hit.position);
    }
    public override void OnStartAuthority()
    {
        mainCamera = Camera.main;
    }

    [ClientCallback]
    private void Update()
    {
        if (!netIdentity.isOwned) return;

        if (!Mouse.current.rightButton.wasPressedThisFrame) return;

        var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return;

        CmdMove(hit.point);
    }
}
