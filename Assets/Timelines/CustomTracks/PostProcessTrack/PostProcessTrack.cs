using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackColor(1f, 0f, 0f)]
[TrackBindingType(typeof(Volume))]
[TrackClipType(typeof(BloomIntensityClip))]
[TrackClipType(typeof(WhiteBalanceClip))]
[TrackClipType(typeof(ColorAdjustmentsClip))]
public class PostProcessTrack : TrackAsset
{

}
