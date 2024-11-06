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
    public virtual string unit => "x";
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
    public virtual void OnWield(Player wielder, InventorySlot slot)
    {
        containedSlot = slot;
        this.wielder = wielder;
    }
    public virtual void OnWieldUpdate() { }
    public virtual void OnUnwield()
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