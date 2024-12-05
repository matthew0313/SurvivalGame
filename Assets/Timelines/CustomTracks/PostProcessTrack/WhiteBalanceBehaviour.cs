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
public class WhiteBalanceBehaviour : PlayableBehaviour
{
    [SerializeField] float temperature, tint;
    bool played = false;
    WhiteBalance whiteBalance;
    float startTemperature, startTint;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        Volume target = playerData as Volume;
        if (target.profile.TryGet(out whiteBalance))
        {
            if (!played)
            {
                played = true;
                startTemperature = whiteBalance.temperature.value;
                startTint = whiteBalance.tint.value;
            }
            whiteBalance.temperature.value = temperature;
            whiteBalance.tint.value = tint;
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        if (played)
        {
            played = false;
            whiteBalance.temperature.value = startTemperature;
            whiteBalance.tint.value = startTint;
        }
    }
}