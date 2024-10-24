using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EquipmentData : ItemData
{
    [Header("Equipment")]
    [SerializeField] float m_maxDurability;
    public float maxDurability => m_maxDurability;
    public override int maxStack => 1;
}
public abstract class Equipment : Item
{
    new EquipmentData data;
    public float maxDurability => data.maxDurability;
    public float durability;
    public Equipment(EquipmentData data) : base(data)
    {
        this.data = data;
        durability = data.maxDurability;
    }
    public override bool IsStackable(Item other) => false;
    public Action onDurabilityChange;
    public void DurabilityReduce(float amount)
    {
        durability = Mathf.Max(0, durability - amount);
        onDurabilityChange?.Invoke();
        if(durability <= 0)
        {
            Debug.Log("Broke");
            containedSlot.count = 0;
        }
    }
}
