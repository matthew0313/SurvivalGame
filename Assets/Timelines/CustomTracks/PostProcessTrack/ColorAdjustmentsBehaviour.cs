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
public class ColorAdjustmentsBehaviour : PlayableBehaviour
{
    [SerializeField] float postExposure, contrast;
    [SerializeField][ColorUsage(false)] Color colorFilter;
    [SerializeField] float hueShift, saturation;
    bool played = false;
    ColorAdjustments colorAdjustments;
    float _postExposure, _contrast, _hueShift, _saturation;
    Color _colorFilter;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        Volume target = playerData as Volume;
        if (target.profile.TryGet(out colorAdjustments))
        {
            if (!played)
            {
                played = true;
                _postExposure = colorAdjustments.postExposure.value;
                _contrast = colorAdjustments.contrast.value;
                _colorFilter = colorAdjustments.colorFilter.value;
                _hueShift = colorAdjustments.hueShift.value;
                _saturation = colorAdjustments.saturation.value;
            }
            colorAdjustments.postExposure.value = postExposure;
            colorAdjustments.contrast.value = contrast;
            colorAdjustments.colorFilter.value = colorFilter;
            colorAdjustments.hueShift.value = hueShift;
            colorAdjustments.saturation.value = saturation;
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        if (played)
        {
            played = false;
            colorAdjustments.postExposure.value = _postExposure;
            colorAdjustments.contrast.value = _contrast;
            colorAdjustments.colorFilter.value = _colorFilter;
            colorAdjustments.hueShift.value = _hueShift;
            colorAdjustments.saturation.value = _saturation;
        }
    }
}