using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackColor(1f, 1f, 0f)]
[TrackBindingType(typeof(Transform))]
[TrackClipType(typeof(MoveToClip))]
[TrackClipType(typeof(SetPositionClip))]
public class PositionTrack : TrackAsset
{

}
