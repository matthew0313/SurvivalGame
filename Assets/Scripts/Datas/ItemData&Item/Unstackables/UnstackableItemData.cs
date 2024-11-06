using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Unstackable Item Data", menuName = "Scriptables/Items/Unstackables/Item", order = 0)]
public class UnstackableItemData : ItemData
{
    public override int maxStack => 1;
    public override Item Create() => new UnstackableItem(this);
}
public class UnstackableItem : Item
{
    new UnstackableItemData data;
    public UnstackableItem(UnstackableItemData data) : base(data)
    {
        this.data = data;
    }
}
