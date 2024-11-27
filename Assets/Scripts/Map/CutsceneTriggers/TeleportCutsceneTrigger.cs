using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

[RequireComponent(typeof(TeleportInteraction))]
public class TeleportCutsceneTrigger : MonoBehaviour, ISavable
{
    [SerializeField] TimelineAsset cutscene;
    [SerializeField] SaveID id;
    TeleportInteraction origin;
    bool played = false;

    private void Awake()
    {
        origin = GetComponent<TeleportInteraction>();
        origin.onTeleport.AddListener(() =>
        {
            if (!played)
            {
                played = true;
                TimelineCutsceneManager.PlayCutscene(cutscene);
            }
        });
    }
    public void Load(SaveData data)
    {
        played = data.mapObjects[id.value].bools["played"];
    }

    public void Save(SaveData data)
    {
        DataUnit tmp = new();
        tmp.bools["played"] = played;
        data.mapObjects[id.value] = tmp;
    }
}