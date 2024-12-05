using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    [SerializeField][TextArea] string dialogue;
    [SerializeField] bool autoSpeed = true;

    [Header("Auto Speed")]
    [SerializeField] float endTime = 1.0f;
    [SerializeField] int pauseUnit = 5;

    [Header("Manual Speed")]
    [SerializeField] float talkSpeed = 0.05f;
    [SerializeField] float pauseTime = 0.25f;

    Talkbox box = null;
    int displayed = 0;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        box = playerData as Talkbox;
        if (box == null) return;
        box.gameObject.SetActive(true);
        float time = (float)playable.GetTime();
        float tot = 0;
        box.text.text = "";
        if(autoSpeed == false)
        {
            for (int i = 0; i < dialogue.Length; i++)
            {
                tot += (dialogue[i] == '.' || dialogue[i] == ',' || dialogue[i] == '!' || dialogue[i] == '?') ? pauseTime : talkSpeed;
                if (time < tot) break;
                box.text.text += dialogue[i];
            }
        }
        else
        {
            float totalTime = (float)playable.GetDuration() - endTime;
            int unitTotal = 0;
            for(int i = 0; i < dialogue.Length; i++)
            {
                unitTotal += (dialogue[i] == '.' || dialogue[i] == ',' || dialogue[i] == '!' || dialogue[i] == '?') ? pauseUnit : 1;
            }
            float timePerUnit = totalTime / unitTotal;
            for (int i = 0; i < dialogue.Length; i++)
            {
                tot += timePerUnit * ((dialogue[i] == '.' || dialogue[i] == ',' || dialogue[i] == '!' || dialogue[i] == '?') ? pauseUnit : 1);
                box.text.text += dialogue[i];
                if (time < tot) break;
            }
        }
        if (box.text.text.Length != displayed)
        {
            box.TalkSound();
            displayed = box.text.text.Length;
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        if (box == null) return;
        else
        {
            box.gameObject.SetActive(false);
        }
    }
}