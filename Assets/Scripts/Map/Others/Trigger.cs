using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;


[RequireComponent(typeof(Collider2D))]
public class Trigger : MonoBehaviour, ICutsceneTriggerReceiver
{
    [SerializeField] UnityEvent trigger;
    Collider2D collider;
    void Awake()
    {
        collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }
    public void OnCutsceneEnter()
    {
        collider.enabled = false;
    }

    public void OnCutsceneExit()
    {
        collider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) trigger?.Invoke();
    }
}