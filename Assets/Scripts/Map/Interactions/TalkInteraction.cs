using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TalkInteraction : Interaction
{
    [SerializeField] Conversation conversation;
    public override bool canInteract => !talking;
    bool talking = false;
    protected override void Awake()
    {
        base.Awake();
        conversation.onConversationEnd.AddListener(EndConversation);
    }
    public override void OnInteract()
    {
        base.OnInteract();
        talking = true;
        conversation.Start();
    }
    void EndConversation() => talking = false;
}