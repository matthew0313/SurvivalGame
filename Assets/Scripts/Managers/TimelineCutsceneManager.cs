using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineCutsceneManager : MonoBehaviour
{
    static TimelineCutsceneManager Instance;
    public TimelineCutsceneManager()
    {
        if (Instance != null) Destroy(this);
        else Instance = this;
    }

    [SerializeField] PlayableDirector director;
    bool m_inCutscene = false;
    public static bool inCutscene => (Instance == null) ? false : Instance.m_inCutscene;
    public static Action onCutsceneEnter, onCutsceneExit;

    private void Awake()
    {
        director.played += (tmp) =>
        {
            m_inCutscene = true;
            onCutsceneEnter?.Invoke();
        };
        director.stopped += (tmp) =>
        {
            m_inCutscene = false;
            onCutsceneExit?.Invoke();
        };
    }
    public static void PlayCutscene(TimelineAsset cutscene)
    {
        if (Instance == null) return;
        Instance.director.playableAsset = cutscene;
        Instance.director.Play();
    }
}