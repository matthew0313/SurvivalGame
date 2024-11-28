using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
using Unity.VisualScripting;

public class Settings : MonoBehaviour
{
    static float m_masterVolume = 1.0f, m_brightness = 1.0f;
    public static float masterVolume
    {
        get
        {
            return m_masterVolume;
        }
        set
        {
            m_masterVolume = value;
            onMasterVolumeChange?.Invoke();
        }
    }
    public static Action onMasterVolumeChange;
    public static float brightness
    {
        get
        {
            return m_brightness;
        }
        set
        {
            m_brightness = value;
            onBrightnessChange?.Invoke();
        }
    }
    public static Action onBrightnessChange;
    public static void Load(SettingsSaveData data)
    {
        masterVolume = data.masterVolume;
        brightness = data.brightness;
    }
    public static SettingsSaveData Save()
    {
        SettingsSaveData data = new();
        data.masterVolume = masterVolume;
        data.brightness = brightness;
        return data;
    }
}
[System.Serializable]
public class SettingsSaveData
{
    public float masterVolume = 1.0f, brightness = 1.0f;
}