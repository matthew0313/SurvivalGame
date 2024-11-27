using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talkbox : MonoBehaviour
{
    [SerializeField] Text m_text;
    [SerializeField] Sound talkSound;
    [SerializeField] GameObject talkSoundSourceParent;
    public Text text => m_text;
    private void Awake()
    {
        TimelineCutsceneManager.onCutsceneEnter += StopAllCoroutines;
    }
    IEnumerator dialoguing = null;
    Action onDialogueFinish = null;
    void OnCutsceneEnter()
    {
        if(dialoguing != null)
        {
            StopCoroutine(dialoguing);
            onDialogueFinish?.Invoke();
        }
    }
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
    public void Dialogue(string dialogue, float talkSpeed, float pauseTime, Action onDialogueFinish)
    {
        this.onDialogueFinish = onDialogueFinish;
        dialoguing = Dialoguing(dialogue, talkSpeed, pauseTime);
        StartCoroutine(dialoguing);
    }
    IEnumerator Dialoguing(string dialogue, float talkSpeed, float pauseTime)
    {
        text.text = "";
        for(int i = 0; i < dialogue.Length; i++)
        {
            text.text += dialogue[i];
            TalkSound();
            yield return new WaitForSeconds((dialogue[i] == '.' || dialogue[i] == ',' || dialogue[i] == '!' || dialogue[i] == '?') ? pauseTime : talkSpeed);
        }
        onDialogueFinish?.Invoke();
        dialoguing = null;
    }
}
