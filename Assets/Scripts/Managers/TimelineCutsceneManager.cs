using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

public class TimelineCutsceneManager : MonoBehaviour, ISavable
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
            foreach (var i in FindObjectsOfType<MonoBehaviour>(true).OfType<ICutsceneTriggerReceiver>()) i.OnCutsceneEnter();
        };
        director.stopped += (tmp) =>
        {
            m_inCutscene = false;
            foreach (var i in FindObjectsOfType<MonoBehaviour>(true).OfType<ICutsceneTriggerReceiver>()) i.OnCutsceneExit();
        };
    }
    public void PlayCutscene(TimelineAsset cutscene)
    {
        fastPlaying = false;
        Instance.director.playableAsset = cutscene;
        Instance.director.Play();
    }
    public bool fastPlaying { get; private set; } = false;
    public void ToggleFastPlay()
    {
        if (fastPlaying)
        {
            fastPlaying = false;
            director.playableGraph.GetRootPlayable(0).SetSpeed(1.0f);
        }
        else
        {
            fastPlaying = true;
            director.playableGraph.GetRootPlayable(0).SetSpeed(3.0f);
        }
    }

    public void Save(SaveData data)
    {
        if (m_inCutscene)
        {
            data.cutscene = director.playableAsset;
            data.cutsceneProgress = (float)director.time;
        }
    }

    public void Load(SaveData data)
    {
        if(data.cutscene != null)
        {
            director.playableAsset = data.cutscene;
            director.time = data.cutsceneProgress;
            director.Play();
        }
    }
}
public interface ICutsceneTriggerReceiver
{
    public void OnCutsceneEnter();
    public void OnCutsceneExit();
}