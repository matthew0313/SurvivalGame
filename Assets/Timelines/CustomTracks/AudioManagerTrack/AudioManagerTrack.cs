using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackColor(1f, 0f, 1f)]
[TrackBindingType(typeof(AudioManager))]
[TrackClipType(typeof(MusicClip))]
[TrackClipType(typeof(SoundClip))]
public class AudioManagerTrack : TrackAsset
{

}
