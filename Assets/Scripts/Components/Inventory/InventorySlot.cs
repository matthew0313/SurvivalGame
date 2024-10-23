using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySlot
{
    Item m_item = null;
    int m_count = 0;

    public Action onItemChange, onCountChange;

    public Func<Item, bool> slotRestriction;
    public Item item { get; private set; }
    public bool SetItem(Item item)
    {
        if (slotRestriction != null && slotRestriction.Invoke(item) == false) return false;
        this.item = item;
        onItemChange?.Invoke();
        return true;
    }
    public int count
    {
        get
        {
            return m_count;
        }
        set
        {
            m_count = value;
            onCountChange?.Invoke();
            if(m_count == 0)
            {
                item = null;
            }
        }
    }
}
