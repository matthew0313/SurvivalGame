using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TalkInteraction : Interaction
{
    [SerializeField] string talkContent;
    [SerializeField] float talkSpeed = 0.05f, pauseTime = 0.25f, endTime = 1.0f;
    [SerializeField] Talkbox talkBox;
    public override string interactText => "대화하기";
    public override bool canInteract => !talking;
    bool talking = false;
    public override void OnInteract()
    {
        base.OnInteract();
        talking = true;
        talkBox.Dialogue(talkContent, talkSpeed, pauseTime, endTime, () => talking = false);
    }
}