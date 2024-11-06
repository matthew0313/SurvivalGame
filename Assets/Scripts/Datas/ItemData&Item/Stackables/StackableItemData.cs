using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Stackable Item Data", menuName = "Scriptables/Items/Stackables/Item", order = 0)]
public class StackableItemData : ItemData
{
    [Header("Stackable")]
    [SerializeField] int m_maxStack = 64;
    [SerializeField] string m_unit = "x";
    public override int maxStack => m_maxStack;
    public override string unit => m_unit;
    public override Item Create() => new StackableItem(this);
}
public class StackableItem : Item
{
    new StackableItemData data;
    public StackableItem(StackableItemData data) : base(data)
    {
        this.data = data;
    }
}
