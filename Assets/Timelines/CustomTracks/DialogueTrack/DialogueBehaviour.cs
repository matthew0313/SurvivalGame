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
    [SerializeField] string dialogue;
    [SerializeField] float endTime = 1.0f;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        Text tmp = playerData as Text;
        if (tmp == null) return;
        float duration = (float)playable.GetDuration() - endTime;
        int totalTicks = 0;
        for(int i = 0; i < dialogue.Length; i++)
        {
            if (dialogue[i] == '.' || dialogue[i] == ',' || dialogue[i] == '!' || dialogue[i] == '?') totalTicks += 5;
            else totalTicks += 1;
        }
        float timePerTick = duration / totalTicks;
        float time = (float)playable.GetTime();
        float tot = 0;
        tmp.text = "";
        for(int i = 0; i < dialogue.Length; i++)
        {
            tmp.text += dialogue[i];
            tot += (dialogue[i] == '.' || dialogue[i] == ',' || dialogue[i] == '!' || dialogue[i] == '?') ? timePerTick * 5 : timePerTick;
            if (time < tot) break;
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
    }
}