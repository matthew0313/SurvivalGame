using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[System.Serializable]
public class SaveData
{
    public float timePlayed = 0;
    public long lastSaved = 0;
    public PlayerSaveData player = new();
    public List<DroppedItemSaveData> droppedItems = new();
    public SerializableDictionary<string, DataUnit> mapObjects = new();
    public SerializableDictionary<string, EnemySpawnSaveData> spawnPoints = new();
    public SerializableDictionary<string, DataUnit> fieldEnemies = new();

    public PlayableAsset cutscene;
    public float cutsceneProgress = 0.0f;
    public Vector2 cameraPos;
}