using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class MoveToClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField] MoveToBehaviour template;
    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<MoveToBehaviour>.Create(graph, template);
    }
}
