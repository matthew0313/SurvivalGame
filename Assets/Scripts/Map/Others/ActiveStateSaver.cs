using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class ActiveStateSaver : MonoBehaviour, ISavable
{
    [SerializeField] SaveID id;
    [SerializeField] bool defaultActiveState = false;
    bool loaded = false;
    void Start()
    {
        if(!loaded) gameObject.SetActive(defaultActiveState);
    }
    public void Load(SaveData data)
    {
        if (data.mapObjects.TryGetValue(id.value, out DataUnit tmp))
        {
            gameObject.SetActive(tmp.bools["active"]);
            loaded = true;
        }
    }

    public void Save(SaveData data)
    {
        DataUnit tmp = new();
        tmp.bools["active"] = gameObject.activeSelf;
        data.mapObjects[id.value] = tmp;
    }
}