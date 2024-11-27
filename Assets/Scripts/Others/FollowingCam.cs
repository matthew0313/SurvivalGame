using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowingCam : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float lerpRate = 0.5f;
    void LateUpdate()
    {
        if (TimelineCutsceneManager.inCutscene) return;
        transform.position = Vector2.Lerp(transform.position, target.position, lerpRate * Time.deltaTime);
        transform.position += new Vector3(0, 0, -10.0f);
    }
}
