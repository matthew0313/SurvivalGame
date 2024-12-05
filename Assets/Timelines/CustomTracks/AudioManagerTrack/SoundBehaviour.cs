using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class SoundBehaviour : PlayableBehaviour
{
    [SerializeField] Sound playingSound;
    bool played = false;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        if (!Application.isPlaying) return;
        if (played) return;
        played = true;
        AudioManager target = playerData as AudioManager;
        target.PlaySound(playingSound);
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        played = false;
    }
}