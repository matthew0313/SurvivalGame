using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioManager() => Instance = this;
    List<AudioSource> sources = new();
    public void PlaySound(Sound sound, Vector2 position)
    {
        AudioSource source = sources.Find((AudioSource tmp) => tmp.isPlaying == false);
        if(source == null)
        {
            source = new GameObject().AddComponent<AudioSource>();
            sources.Add(source);
        }
        source.volume = sound.volume * Settings.masterVolume;
        source.clip = sound.clip;
        source.transform.position = position;
        source.Play();
    }
    public void PlaySound(Sound sound, Transform positioner) => PlaySound(sound, positioner.position);
}