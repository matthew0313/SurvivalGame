using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;

public class Talkbox : MonoBehaviour, ICutsceneTriggerReceiver
{
    [SerializeField] TMP_Text m_text;
    [SerializeField] Sound talkSound;
    public TMP_Text text => m_text;
    ITalkable currentTalk;
    IEnumerator dialoguing = null;
    List<AudioSource> talkSoundSources = new();
    public void TalkSound()
    {
        AudioManager.Instance.PlaySound(talkSound);
    }
    public void Dialogue(ITalkable talk)
    {
        if(dialoguing != null)
        {
            if (currentTalk.priority > talk.priority || currentTalk == talk) return;
            else
            {
                currentTalk.Interrupt();
                StopCoroutine(dialoguing);
            }
        }
        gameObject.SetActive(true);
        dialoguing = Dialoguing(talk);
        StartCoroutine(dialoguing);
    }
    IEnumerator Dialoguing(ITalkable talk)
    {
        DialogueLine line = talk.GetDialogue();
        currentTalk = talk;
        text.text = "";
        for (int i = 0; i < line.dialogue.Length; i++)
        {
            text.text += line.dialogue[i];
            TalkSound();
            yield return new WaitForSeconds((line.dialogue[i] == '.' || line.dialogue[i] == ',' || line.dialogue[i] == '!' || line.dialogue[i] == '?') ? line.pauseTime : line.talkSpeed);
        }
        yield return new WaitForSeconds(line.endTime);
        gameObject.SetActive(false);
        dialoguing = null;
        currentTalk = null;
        talk.End();
    }

    public void OnCutsceneEnter()
    {
        if (dialoguing != null)
        {
            currentTalk.Interrupt();
            StopCoroutine(dialoguing);
            gameObject.SetActive(false);
        }
    }

    public void OnCutsceneExit() { }
}
public interface ITalkable
{
    public int priority { get; }
    public DialogueLine GetDialogue();
    public void End();
    public void Interrupt();
}
[System.Serializable]
public class Conversation : ITalkable
{
    [SerializeField] ConversationElement[] elements;
    [SerializeField] int m_priority;
    public UnityEvent onConversationEnd;
    public int priority => m_priority;
    int current;
    public void Start()
    {
        current = -1;
        End();
    }
    public void End()
    {
        if (current >= 0) elements[current].line.End();
        if (++current >= elements.Length)
        {
            onConversationEnd?.Invoke();
            return;
        }
        elements[current].box.Dialogue(this);
    }
    public void Interrupt()
    {
        for(int i = current; i < elements.Length; i++)
        {
            elements[i].line.Interrupt();
        }
        onConversationEnd?.Invoke();
    }
    public DialogueLine GetDialogue()
    {
        return elements[current].line;
    }
}
[System.Serializable]
public struct ConversationElement
{
    public Talkbox box;
    public DialogueLine line;
}
[System.Serializable]
public class DialogueLine : ITalkable
{
    [SerializeField] string m_dialogue;
    [SerializeField] float m_talkSpeed, m_pauseTime, m_endTime;
    [SerializeField] int m_priority;
    public UnityEvent onDialogueEnd;
    public string dialogue => m_dialogue;
    public float talkSpeed => m_talkSpeed;
    public float pauseTime => m_pauseTime;
    public float endTime => m_endTime;
    public int priority => m_priority;
    public DialogueLine GetDialogue() => this;
    public void End()
    {
        onDialogueEnd?.Invoke();
    }
    public void Interrupt()
    {
        onDialogueEnd?.Invoke();
    }

}