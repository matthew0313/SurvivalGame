using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Talkbox : MonoBehaviour, ICutsceneTriggerReceiver
{
    [SerializeField] TMP_Text m_text;
    [SerializeField] Sound talkSound;
    public TMP_Text text => m_text;
    IEnumerator dialoguing = null;
    List<AudioSource> talkSoundSources = new();
    public void TalkSound()
    {
        AudioManager.Instance.PlaySound(talkSound);
    }
    public void Dialogue(string[] dialogues, float talkSpeed, float pauseTime, float endTime, Action onDialogueFinish)
    {
        gameObject.SetActive(true);
        dialoguing = Dialoguing(dialogues, talkSpeed, pauseTime, endTime, onDialogueFinish);
        StartCoroutine(dialoguing);
    }
    IEnumerator Dialoguing(string[] dialogues, float talkSpeed, float pauseTime, float endTime, Action onDialogueFinish)
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
        if (dialoguing != null) StopCoroutine(dialoguing);
    }

    public void OnCutsceneExit() { }
}