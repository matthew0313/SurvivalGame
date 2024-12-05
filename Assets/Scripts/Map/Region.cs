using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Timeline;


public class Region : MonoBehaviour
{
    static Region currentRegion;

    [SerializeField] Sound music;
    MusicPriorityPair musicPriorityPair;
    private void Awake()
    {
        musicPriorityPair = new MusicPriorityPair() { music = music, priority = 1 };
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (currentRegion != null) currentRegion.UnsetRegion();
            currentRegion = this;
            currentRegion.SetRegion();
        }
    }
    void UnsetRegion()
    {
        AudioManager.Instance.RemoveMusic(musicPriorityPair);
    }
    void SetRegion()
    {
        AudioManager.Instance.AddMusic(musicPriorityPair);
    }
}