using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

[System.Serializable]
public class SaveData
{
    public PlayerSaveData player;
    public SaveData()
    {
        player = new();
    }
}