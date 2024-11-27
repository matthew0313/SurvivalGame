using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class SetPositionClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField] SetPositionBehaviour template;
    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<SetPositionBehaviour>.Create(graph, template);
    }
}
