using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportInteraction : Interaction
{
    [SerializeField] string m_interactText = "Enter";
    [SerializeField] Vector2 teleportDestination;
    Player player;
    public override string interactText => m_interactText;
    public override bool removeUponInteract => true;
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    public override void OnInteract()
    {
        base.OnInteract();
        player.Teleport(teleportDestination);
    }
}