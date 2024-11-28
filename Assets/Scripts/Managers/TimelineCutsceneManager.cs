using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

public class TimelineCutsceneManager : MonoBehaviour
{
    public static TimelineCutsceneManager Instance { get; private set; }
    public TimelineCutsceneManager()
    {
        Instance = this;
    }

    [SerializeField] PlayableDirector director;
    bool m_inCutscene = false;
    public static bool inCutscene => (Instance == null) ? false : Instance.m_inCutscene;

    private void Awake()
    {
        director.played += (tmp) =>
        {
            m_inCutscene = true;
            foreach (var i in FindObjectsOfType<MonoBehaviour>().OfType<ICutsceneTriggerReceiver>()) i.OnCutsceneEnter();
        };
        director.stopped += (tmp) =>
        {
            m_inCutscene = false;
            foreach (var i in FindObjectsOfType<MonoBehaviour>().OfType<ICutsceneTriggerReceiver>()) i.OnCutsceneExit();
        };
    }
    public static void PlayCutscene(TimelineAsset cutscene)
    {
        if (Instance == null) return;
        Instance.director.playableAsset = cutscene;
        Instance.director.Play();
    }
}
public interface ICutsceneTriggerReceiver
{
    public void OnCutsceneEnter();
    public void OnCutsceneExit();
}