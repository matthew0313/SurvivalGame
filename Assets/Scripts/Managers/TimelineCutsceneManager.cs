using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineCutsceneManager : MonoBehaviour
{
    static TimelineCutsceneManager Instance;
    public TimelineCutsceneManager() => Instance = this;


    bool m_inCutscene = false;
    public static bool inCutscene => (Instance == null) ? false : Instance.m_inCutscene;
    public static Action onCutsceneEnter, onCutsceneExit;
}