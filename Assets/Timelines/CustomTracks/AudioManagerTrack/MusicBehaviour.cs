using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class MusicBehaviour : PlayableBehaviour
{
    [SerializeField] Sound changedMusic;
    [SerializeField] int priority = 10;
    AudioManager target;
    MusicPriorityPair music;
    bool added = false;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        if (!Application.isPlaying) return;
        if (added) return;
        added = true;
        target = playerData as AudioManager;
        if (music == null) music = new MusicPriorityPair();
        music.music = changedMusic;
        music.priority = priority;
        target.AddMusic(music);
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        if (!added) return;
        added = false;
        target.RemoveMusic(music);
    }
}