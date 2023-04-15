using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [field : SerializeField] public Transform AimAtPoint { get; set; }
}
