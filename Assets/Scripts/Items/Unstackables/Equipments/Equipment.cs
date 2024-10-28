using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EquipmentData : UnstackableItemData
{
    [Header("Equipment")]
    [SerializeField] float m_maxDurability;
    public float maxDurability => m_maxDurability;
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
            OnBreak();
            containedSlot.count = 0;
        }
    }
    protected virtual void OnBreak()
    {

    }
    public override Item Copy()
    {
        Equipment item = base.Copy() as Equipment;
        item.durability = durability;
        return item;
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats.Add("Durability", durability);
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        if (data.floats.TryGetValue("Durability", out float tmp))
        {
            durability = tmp;
            onDurabilityChange?.Invoke();
        }
    }
}
