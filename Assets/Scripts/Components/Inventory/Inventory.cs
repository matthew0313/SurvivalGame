using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;

public class Inventory
{
    public readonly InventorySlot[] slots;
    public Inventory(int slotCount)
    {
        slots = new InventorySlot[slotCount];
        for(int i = 0; i < slotCount; i++) slots[i] = new InventorySlot();
    }
    public int Insert(Item item, int count)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].count = Mathf.Min(item.data.maxStack, slots[i].count + count);
                count -= slots[i].count;
            }
            else if (slots[i].item.IsStackable(item) && item.IsStackable(slots[i].item))
            {
                int prev = slots[i].count;
                slots[i].count = Mathf.Min(slots[i].item.data.maxStack, slots[i].count + count);
                count -= slots[i].count - prev;
            }
            if (count <= 0) return 0;
        }
        return count;
    }
    public int Search(ItemData data)
    {
        int tot = 0;
        foreach(var i in slots)
        {
            if(i.item != null && i.item.data == data)
            {
                tot += i.count;
            }
        }
        return tot;
    }
    public bool TakeOut(ItemData data, int count)
    {
        if (Search(data) < count) return false;
        foreach(var i in slots)
        {
            if (i.item != null && i.item.data == data)
            {
                int prev = count;
                count = Mathf.Max(0, count - i.count);
                i.count -= prev - count;
                if (count <= 0) break; 
            }
        }
        return true;
    }
}
