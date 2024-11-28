using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioManager() => Instance = this;
    List<(AudioSource, Sound)> sources = new();
    List<(AudioSource, Transform)> tracedSources = new();
    [SerializeField] Sound currentMusic;
    [SerializeField] AudioSource musicPlayer;
    private void Awake()
    {
        Settings.onMasterVolumeChange += OnVolumeChange;
        musicPlayer.volume = currentMusic.volume * Settings.masterVolume;
        musicPlayer.clip = currentMusic.clip;
        musicPlayer.Play();
    }
    void OnVolumeChange()
    {
        musicPlayer.volume = currentMusic.volume * Settings.masterVolume;
        foreach (var i in sources) i.Item1.volume = i.Item2.volume * Settings.masterVolume;
    }
    List<(AudioSource, Transform)> removeQueue = new();
    private void Update()
    {
        foreach (var i in tracedSources)
        {
            if (i.Item2 != null) i.Item1.transform.position = i.Item2.position;
            if (i.Item1.isPlaying == false) removeQueue.Add(i);
        }
        foreach (var i in removeQueue) tracedSources.Remove(i);
    }
    public void PlaySound(Sound sound, Vector2 position)
    {
        AudioSource source = sources.Find((tmp) => tmp.Item1.isPlaying == false).Item1;
        if(source == null)
        {
            source = new GameObject().AddComponent<AudioSource>();
            sources.Add((source, sound));
        }
        source.volume = sound.volume * Settings.masterVolume;
        source.clip = sound.clip;
        source.transform.position = position;
        source.Play();
    }
    public void PlaySound(Sound sound, Transform positioner)
    {
        AudioSource source = sources.Find((tmp) => tmp.Item1.isPlaying == false).Item1;
        if (source == null)
        {
            source = new GameObject().AddComponent<AudioSource>();
            sources.Add((source, sound));
        }
        source.volume = sound.volume * Settings.masterVolume;
        source.clip = sound.clip;
        source.transform.position = positioner.position;
        tracedSources.Add((source, positioner));
        source.Play();
    }
    public void ChangeMusic(Sound music)
    {
        currentMusic = music;
        musicPlayer.volume = currentMusic.volume * Settings.masterVolume;
        musicPlayer.clip = currentMusic.clip;
        musicPlayer.Play();
    }
    public void FadeoutMusic(Action onFadeoutEnd)
    {
        StartCoroutine(FadingoutMusic(onFadeoutEnd));
    }
    const float fadeTime = 2.0f;
    IEnumerator FadingoutMusic(Action onFadeoutEnd)
    {
        float start = musicPlayer.volume;
        float counter = 0.0f;
        while(counter < fadeTime)
        {
            counter = Mathf.Min(fadeTime, counter + Time.deltaTime);
            musicPlayer.volume = Mathf.Lerp(start, 0.0f, counter / fadeTime);
            yield return null;
        }
        onFadeoutEnd?.Invoke();
    }
    private void OnDestroy()
    {
        Settings.onMasterVolumeChange -= OnVolumeChange;
    }
}