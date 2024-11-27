using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour, ISavable
{
    [Header("Spawning")]
    [SerializeField] EnemyChancePair[] spawns;
    [SerializeField] TimeVal spawnCooldown;
    [SerializeField] int maxSpawned;
    [SerializeField] float spawnRadius;

    [Space(10)]
    [SerializeField] SaveID id;

    float counter = 0.0f;
    List<Enemy> spawned = new();
    private void OnValidate()
    {
        if (!gameObject.scene.IsValid()) id.value = null;
        else if (string.IsNullOrEmpty(id.value)) id.SetNew();
    }
    void Start()
    {
        FullySpawn();
    }
    void Update()
    {
        if (spawned.Count < maxSpawned)
        {
            counter += Time.deltaTime;
            if(counter >= spawnCooldown.time)
            {
                SpawnEnemy();
                counter = 0.0f;
            }
        }
        else counter = 0.0f;
    }
    public void SpawnEnemy()
    {
        Vector2 pos = (Vector2)transform.position + Utilities.RandomAngle(0, 360) * UnityEngine.Random.Range(0, spawnRadius);
        Enemy spawning = null;
        float tot = 0;
        foreach (var i in spawns) tot += i.chance;
        float rand = UnityEngine.Random.Range(0, tot);
        tot = 0;
        for(int i = 0; i < spawns.Length; i++)
        {
            if (rand <= spawns[i].chance + tot)
            {
                spawning = spawns[i].enemy;
                break;
            }
            else tot += spawns[i].chance;
        }
        Enemy spawned = spawning.Instantiate();
        spawned.transform.position = pos;
        this.spawned.Add(spawned);
        spawned.onDeath.AddListener(() => this.spawned.Remove(spawned));
    }
    public void FullySpawn()
    {
        while (spawned.Count < maxSpawned) SpawnEnemy();
    }
    public void Load(SaveData data)
    {
        if(data.spawnPoints.TryGetValue(id.value, out EnemySpawnSaveData tmp))
        {
            foreach(var i in tmp.enemies)
            {
                Enemy spawned = i.prefab.Instantiate();
                this.spawned.Add(spawned);
                spawned.onDeath.AddListener(() => this.spawned.Remove(spawned));
                spawned.Load(i.data);
            }
            counter = tmp.counter;
        }
    }

    public void Save(SaveData data)
    {
        EnemySpawnSaveData tmp = new();
        foreach(var i in spawned)
        {
            EnemySaveData tmp2 = new();
            tmp2.prefab = i.prefabOrigin;
            tmp2.data = i.Save();
            tmp.enemies.Add(tmp2);
        }
        tmp.counter = counter;
        data.spawnPoints[id.value] = tmp;
    }
}
[System.Serializable]
public class EnemySpawnSaveData
{
    public List<EnemySaveData> enemies = new();
    public float counter = 0.0f;
}
[System.Serializable]
public struct EnemyChancePair
{
    [SerializeField] Enemy m_enemy;
    [SerializeField] float m_chance;
    public Enemy enemy => m_enemy;
    public float chance => m_chance;
}
