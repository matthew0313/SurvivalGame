using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talkbox : MonoBehaviour, ICutsceneTriggerReceiver
{
    [SerializeField] Text m_text;
    [SerializeField] Sound talkSound;
    [SerializeField] GameObject talkSoundSourceParent;
    public Text text => m_text;
    IEnumerator dialoguing = null;
    Action onDialogueFinish = null;
    List<AudioSource> talkSoundSources = new();
    public void TalkSound()
    {
        AudioSource tmp = talkSoundSources.Find((source) => source.isPlaying == false);
        if (tmp == null)
        {
            if (talkSoundSourceParent == null) return;
            tmp = talkSoundSourceParent.AddComponent<AudioSource>();
            tmp.playOnAwake = false;
        }
        tmp.clip = talkSound.clip;
        tmp.volume = talkSound.volume;
        tmp.Play();
    }
    public void Dialogue(string[] dialogues, float talkSpeed, float pauseTime, float endTime, Action onDialogueFinish)
    {
        gameObject.SetActive(true);
        this.onDialogueFinish = onDialogueFinish;
        dialoguing = Dialoguing(dialogues, talkSpeed, pauseTime, endTime);
        StartCoroutine(dialoguing);
    }
    IEnumerator Dialoguing(string[] dialogues, float talkSpeed, float pauseTime, float endTime)
    {
        for(int i = 0; i < dialogues.Length; i++)
        {
            text.text = "";
            for (int k = 0; k < dialogues[i].Length; k++)
            {
                text.text += dialogues[i][k];
                TalkSound();
                yield return new WaitForSeconds((dialogues[i][k] == '.' || dialogues[i][k] == ',' || dialogues[i][k] == '!' || dialogues[i][k] == '?') ? pauseTime : talkSpeed);
            }
            yield return new WaitForSeconds(endTime);
        }
        gameObject.SetActive(false);
        onDialogueFinish?.Invoke();
        dialoguing = null;
    }

    public void OnCutsceneEnter()
    {
        if (dialoguing != null)
        {
            StopCoroutine(dialoguing);
            onDialogueFinish?.Invoke();
        }
    }

    public void OnCutsceneExit() { }
}