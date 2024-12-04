using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TalkInteraction : Interaction
{
    [SerializeField][TextArea] string[] talkContent;
    [SerializeField] float talkSpeed = 0.05f, pauseTime = 0.25f, endTime = 1.0f;
    [SerializeField] Talkbox talkBox;
    public Action onTalkStart, onTalkEnd;
    public override bool canInteract => !talking;
    bool talking = false;
    public override void OnInteract()
    {
        base.OnInteract();
        talking = true;
        talkBox.Dialogue(talkContent, talkSpeed, pauseTime, endTime, () => talking = false);
    }
}