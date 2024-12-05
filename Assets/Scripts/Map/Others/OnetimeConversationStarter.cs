using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class OnetimeConversationStarter : MonoBehaviour, ISavable
{
    [SerializeField] ConversationElement[] conversation;
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

    public void StartConversation()
    {
        if (done) return;
        done = true;
        Progress(0);
    }
    void Progress(int step)
    {
        conversation[step].talkbox.Dialogue(conversation[step].dialogue, 0.05f, 0.25f, 1.0f, step == conversation.Length-1 ? null : () => Progress(step + 1));
    }
}
[System.Serializable]
public struct ConversationElement
{
    [SerializeField] Talkbox m_talkbox;
    [SerializeField][TextArea] string[] m_dialogue;
    public Talkbox talkbox => m_talkbox;
    public string[] dialogue => m_dialogue;
}