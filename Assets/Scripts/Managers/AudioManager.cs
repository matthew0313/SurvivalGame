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
    [SerializeField] Sound defaultMusic;
    [SerializeField] AudioSource musicPlayer;
    List<MusicPriorityPair> musicPriorityList = new();
    MusicPriorityPair currentMusic;
    private void Awake()
    {
        Settings.onMasterVolumeChange += OnVolumeChange;
        currentMusic = new MusicPriorityPair() { music = defaultMusic, priority = 0 };
        musicPriorityList.Add(currentMusic);
        musicPlayer.clip = currentMusic.music.clip;
        musicPlayer.volume = currentMusic.music.volume * Settings.masterVolume;
        musicPlayer.Play();
        RefreshMusic();
    }
    void OnVolumeChange()
    {
        musicPlayer.volume = currentMusic.music.volume * Settings.masterVolume;
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
    public void PlaySound(Sound sound)
    {
        if (!Application.isPlaying) return;
        AudioSource source = sources.Find((tmp) => tmp.Item1.isPlaying == false).Item1;
        if(source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            sources.Add((source, sound));
        }
        source.volume = sound.volume * Settings.masterVolume;
        source.clip = sound.clip;
        source.Play();
    }
    public Action<AudioClip> onMusicChange;
    public void RefreshMusic()
    {
        musicPriorityList.Sort((a, b) => b.priority.CompareTo(a.priority));
        AudioClip prev = currentMusic.music.clip;
        currentMusic = musicPriorityList[0];
        if (currentMusic.music.clip != prev)
        {
            FadeMusic(1.0f, () =>
            {
                musicPlayer.clip = currentMusic.music.clip;
                musicPlayer.volume = currentMusic.music.volume * Settings.masterVolume;
                if(musicPlayer.clip != null) musicPlayer.Play();
                onMusicChange?.Invoke(musicPlayer.clip);
            });
        }
        else musicPlayer.volume = currentMusic.music.volume * Settings.masterVolume;
    }
    public void AddMusic(MusicPriorityPair music)
    {
        if (!musicPriorityList.Contains(music))
        {
            musicPriorityList.Add(music);
        }
        RefreshMusic();
    }
    public void RemoveMusic(MusicPriorityPair music)
    {
        if(musicPriorityList.Contains(music))
        {
            musicPriorityList.Remove(music);
        }
        RefreshMusic();
    }
    public bool ContainsMusic(MusicPriorityPair music) => musicPriorityList.Contains(music);
    IEnumerator fadingMusic = null;
    public void FadeMusic(float fadeTime, Action onFadeEnd)
    {
        if (fadingMusic != null)
        {
            StopCoroutine(fadingMusic);
        }
        fadingMusic = FadingMusic(fadeTime, onFadeEnd);
        StartCoroutine(fadingMusic);
    }
    IEnumerator FadingMusic(float fadeTime, Action onFadeEnd)
    {
        float start = musicPlayer.volume;
        float counter = 0.0f;
        while(counter < fadeTime)
        {
            counter += Time.deltaTime;
            musicPlayer.volume = start * (1.0f - Mathf.Max(0.0f, counter / fadeTime));
            yield return null;
        }
        fadingMusic = null;
        onFadeEnd?.Invoke();
    }
    private void OnDestroy()
    {
        Settings.onMasterVolumeChange -= OnVolumeChange;
    }
}
[System.Serializable]
public class MusicPriorityPair
{
    public Sound music;
    public int priority;
}