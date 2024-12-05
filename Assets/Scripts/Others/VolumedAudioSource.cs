using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class VolumedAudioSource : MonoBehaviour
{
    AudioSource origin;
    private void Awake()
    {
        origin = GetComponent<AudioSource>();
        origin.volume = Settings.masterVolume;
        Settings.onMasterVolumeChange += OnVolumeChange;
    }
    void OnVolumeChange()
    {
        origin.volume = Settings.masterVolume;
    }
    private void OnDestroy()
    {
        Settings.onMasterVolumeChange -= OnVolumeChange;
    }
}