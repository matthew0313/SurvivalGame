using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackColor(0.8f, 0.8f, 0.8f)]
[TrackBindingType(typeof(Text))]
[TrackClipType(typeof(DialogueClip))]
public class DialogueTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        foreach(var clip in GetClips())
        {

        }
        return base.CreateTrackMixer(graph, go, inputCount);
    }
}
