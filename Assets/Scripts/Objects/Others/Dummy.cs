using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(HpComp), typeof(HpEffects))]
public class Dummy : MonoBehaviour, ISavable
{
    [SerializeField] SaveID id;
    HpComp hp;
    private void Awake()
    {
        hp = GetComponent<HpComp>();
        hp.onDeath += () => { hp.Revive(); hp.FullHeal(); };
    }
    private void OnValidate()
    {
        if (!gameObject.scene.IsValid()) id.value = null;
        else if (string.IsNullOrEmpty(id.value)) id.SetNew();
    }
    public void Load(SaveData data)
    {
        DataUnit tmp = data.mapObjects[id.value];
        hp.Load(JsonUtility.FromJson<HpCompSaveData>(tmp.strings["hp"]));
    }
    public void Save(SaveData data)
    {
        DataUnit tmp = new();
        tmp.strings["hp"] = JsonUtility.ToJson(hp.Save());
        data.mapObjects[id.value] = tmp;
    }
}