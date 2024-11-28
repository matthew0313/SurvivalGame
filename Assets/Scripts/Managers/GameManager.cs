using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, ISavable
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;
    public static bool isInGame => Instance != null;
    public void ReturnToTitle()
    {
        GlobalManager.Instance.Save("AutoSave");
        Time.timeScale = 1.0f;
        GlobalManager.Instance.LoadScene("Title");
    }
    [NonSerialized] public bool canTogglePause = true;
    public bool paused { get; private set; } = false;
    public float timePlayed { get; private set; } = 0;
    public Action onPauseToggle;
    public void Pause()
    {
        if (!canTogglePause) return;
        if (paused)
        {
            paused = false;
            Time.timeScale = 1.0f;
        }
        else
        {
            paused = true;
            Time.timeScale = 0.0f;
        }
        onPauseToggle?.Invoke();
    }
    void Start()
    {
        GlobalManager.Instance.LoadIngame();
    }
    private void Update()
    {
        timePlayed += Time.deltaTime;
        if (!DeviceManager.IsMobile())
        {
            if (Input.GetKeyDown(KeyCode.E)) UIManager.Instance.InventoryTab();
            if (Input.GetKeyDown(KeyCode.Escape)) Pause();
        }
    }
    private void OnDestroy()
    {
        Instance = null;
    }

    public void Save(SaveData data)
    {
        data.lastSaved = DateTime.Now.Ticks;
        data.timePlayed = timePlayed;
    }

    public void Load(SaveData data)
    {
        timePlayed = data.timePlayed;
    }
}