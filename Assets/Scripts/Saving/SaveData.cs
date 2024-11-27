using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

[System.Serializable]
public class SaveData
{
    public PlayerSaveData player = new();
    public List<DroppedItemSaveData> droppedItems = new();
    public SerializableDictionary<string, DataUnit> mapObjects = new();
    public SerializableDictionary<string, EnemySpawnSaveData> spawnPoints = new();
    public SerializableDictionary<string, DataUnit> fieldEnemies = new();
}