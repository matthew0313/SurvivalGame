using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] CutsceneDirectorBindings[] directorBindings;
    Dictionary<int, PlayableDirector> directors = new();
    private void Awake()
    {
        foreach (var i in directorBindings) directors.Add(Animator.StringToHash(i.stateName), i.director);
        foreach(var i in anim.GetBehaviours<CutsceneSegment>())
        {
            i.playDirector += PlayDirector;
        }
    }
    void PlayDirector(int hash)
    {
        if (!directors.ContainsKey(hash)) return;
        directors[hash].Play();
    }
}
[System.Serializable]
public struct CutsceneDirectorBindings
{
    [SerializeField] string m_stateName;
    [SerializeField] PlayableDirector m_director;
    public string stateName => m_stateName;
    public PlayableDirector director => m_director;
}