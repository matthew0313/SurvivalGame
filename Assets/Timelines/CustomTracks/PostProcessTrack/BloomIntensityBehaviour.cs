using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class BloomIntensityBehaviour : PlayableBehaviour
{
    [SerializeField] float intensity, changeTime;
    [SerializeField] bool instant = true;
    bool played = false;
    Bloom bloom;
    float startIntensity;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        Volume target = playerData as Volume;
        if (target.profile.TryGet(out bloom))
        {
            if (!played)
            {
                played = true;
                startIntensity = bloom.intensity.value;
            }
            if (instant)
            {
                bloom.intensity.value = intensity;
            }
            else
            {
                bloom.intensity.value = Mathf.Lerp(startIntensity, intensity, Mathf.Min(1.0f, (float)playable.GetTime() / changeTime));
            }
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        if (played)
        {
            played = false;
            bloom.intensity.value = startIntensity;
        }
    }
}