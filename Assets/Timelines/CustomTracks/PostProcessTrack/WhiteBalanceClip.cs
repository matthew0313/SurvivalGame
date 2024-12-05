using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class WhiteBalanceClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField] WhiteBalanceBehaviour template;
    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<WhiteBalanceBehaviour>.Create(graph, template);
    }
}
