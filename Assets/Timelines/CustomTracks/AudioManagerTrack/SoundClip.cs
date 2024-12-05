using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class SoundClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField] SoundBehaviour template;
    public ClipCaps clipCaps => ClipCaps.Extrapolation;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<SoundBehaviour>.Create(graph, template);
    }
}
