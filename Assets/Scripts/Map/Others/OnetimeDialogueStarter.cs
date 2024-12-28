using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class OnetimeDialogueStarter : MonoBehaviour, ISavable
{
    [SerializeField] DialogueLine dialogue;
    [SerializeField] Talkbox target;
    [SerializeField] SaveID id;
    bool done = false;

    public void Load(SaveData data)
    {
        if (data.mapObjects.TryGetValue(id.value, out DataUnit tmp))
        {
            done = tmp.bools["done"];
        }
    }

    public void Save(SaveData data)
    {
        DataUnit tmp = new();
        tmp.bools["done"] = done;
        data.mapObjects[id.value] = tmp;
    }

    public void StartDialogue()
    {
        if (done) return;
        done = true;
        target.Dialogue(dialogue);
    }
}