using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class OnetimeCutscene : MonoBehaviour, ISavable
{
    [SerializeField] TimelineAsset cutscene;
    [SerializeField] SaveID id;
    [SerializeField] bool playOnStart = false;
    bool played = false;
    void Start()
    {
        if (playOnStart) PlayCutscene();
    }
    public void PlayCutscene()
    {
        if (played) return;
        played = true;
        TimelineCutsceneManager.PlayCutscene(cutscene);
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