using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptables/Items/Item", order = 0)]
public class ItemData : ScriptableObject
{
    [Header("Item")]
    [SerializeField] Sprite m_image;
    public Sprite image => m_image;
    public virtual Item Create() => new Item(this);
    public virtual int maxStack => 99;
}
public class Item
{
    public readonly ItemData data;
    public Item(ItemData data)
    {
        this.data = data;
    }
    public virtual bool IsStackable(Item other) => data == other.data;
    public virtual Item Copy() => data.Create();
    public InventorySlot containedSlot { get; private set; } = null;
    public virtual void OnWield(Player origin, InventorySlot slot)
    {
        containedSlot = slot;
    }
    public virtual void OnWieldUpdate(Player origin) { }
    public virtual void OnUnwield(Player origin) { }
    public virtual float DescBarFill() => 0.0f;
    public virtual string DescBar() => "";
    public Action onDescUpdate;
}
