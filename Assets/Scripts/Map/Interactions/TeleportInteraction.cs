using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TeleportInteraction : Interaction
{
    [SerializeField] Vector2 teleportDestination;
    public UnityEvent onTeleport;
    Player player;
    public override bool removeUponInteract => true;
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    public override void OnInteract()
    {
        base.OnInteract();
        UIManager.Instance.BlackOut(Teleport);
    }
    void Teleport()
    {
        player.Teleport(teleportDestination);
        onTeleport?.Invoke();
    }

}