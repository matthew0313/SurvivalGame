using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Loot Table", menuName = "Scriptables/Loot Table", order = 1)]
public class LootTable : ScriptableObject
{
    [SerializeField] List<LootGroup> table = new();
    [SerializeField] LootGroupSelection lootGroupSelection;
    public IEnumerable<ItemIntPair> GenerateLoot()
    {
        if(lootGroupSelection == LootGroupSelection.RandomIndex)
        {
            int index = UnityEngine.Random.Range(0, table.Count);
            foreach (var k in table[index].elements)
            {
                if (UnityEngine.Random.Range(0.0f, 100.0f) <= k.chance)
                {
                    yield return new ItemIntPair() { item = k.item.Create(), count = UnityEngine.Random.Range(k.minCount, k.maxCount + 1) };
                }
            }
        }
        else if(lootGroupSelection == LootGroupSelection.ChanceForEach)
        {
            foreach (var i in table)
            {
                if (UnityEngine.Random.Range(0.0f, 100.0f) <= i.chance)
                {
                    foreach (var k in i.elements)
                    {
                        if (UnityEngine.Random.Range(0.0f, 100.0f) <= k.chance)
                        {
                            yield return new ItemIntPair() { item = k.item.Create(), count = UnityEngine.Random.Range(k.minCount, k.maxCount + 1) };
                        }
                    }
                }
            }
        }
    }
}
[System.Serializable]
public enum LootGroupSelection
{
    RandomIndex,
    ChanceForEach
}
[System.Serializable]
public struct LootGroup
{
    [SerializeField] float m_chance;
    [SerializeField] LootElement[] m_elements;
    public float chance => m_chance;
    public LootElement[] elements => m_elements;
}
[System.Serializable]
public struct LootElement
{
    [SerializeField] ItemData m_item;
    [SerializeField] float m_chance;
    [SerializeField] int m_minCount, m_maxCount;
    public ItemData item => m_item;
    public float chance => m_chance;
    public int minCount => m_minCount;
    public int maxCount => m_maxCount;
}