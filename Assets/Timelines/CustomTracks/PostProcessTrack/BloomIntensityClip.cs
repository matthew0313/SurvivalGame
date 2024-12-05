using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class BloomIntensityClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField] BloomIntensityBehaviour template;
    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<BloomIntensityBehaviour>.Create(graph, template);
    }
}