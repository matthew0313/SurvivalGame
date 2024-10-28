using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ItemData : ScriptableObject
{
    [Header("Item")]
    [SerializeField] Sprite m_image;
    public Sprite image => m_image;
    public abstract Item Create();
    public abstract int maxStack { get; }
}
public abstract class Item
{
    public readonly ItemData data;
    public Item(ItemData data)
    {
        this.data = data;
    }
    public virtual bool IsStackable(Item other) => data == other.data;
    public virtual Item Copy() => data.Create();
    public InventorySlot containedSlot { get; private set; } = null;
    public Player wielder { get; private set; } = null;
    public virtual void OnWield(Player origin, InventorySlot slot)
    {
        containedSlot = slot;
        wielder = origin;
    }
    public virtual void OnWieldUpdate(Player origin) { }
    public virtual void OnUnwield(Player origin)
    {
        containedSlot = null;
        wielder = null;
    }
    public virtual float DescBarFill() => 0.0f;
    public virtual string DescBar() => "";
    public Action onDescUpdate;
    public virtual void Save(DataUnit data) { }
    public virtual void Load(DataUnit data) { }
}