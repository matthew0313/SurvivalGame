using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[System.Serializable]
public class MoveToBehaviour : PlayableBehaviour
{
    [SerializeField] Vector2 targetPos;
    [SerializeField] float z;
    [SerializeField] AnimationCurve moveCurve;
    Vector2 startPos;
    bool firstFramePlayed = false;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        Transform target = playerData as Transform;
        if (target == null) return;
        if (!firstFramePlayed)
        {
            startPos = target.position;
            firstFramePlayed = true;
        }
        target.position = Vector2.Lerp(startPos, targetPos, moveCurve.Evaluate((float)playable.GetTime() / (float)playable.GetDuration()));
        target.position += new Vector3(0, 0, z);
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        firstFramePlayed = false;
    }
}