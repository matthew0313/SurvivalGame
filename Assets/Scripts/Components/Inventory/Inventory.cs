using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField] int defaultSlotCount = 36;
    public List<InventorySlot> slots { get; } = new();
    public Action<ItemData> onInventoryUpdate;
    int m_slotCount = 0;
    public Action onSlotCountChange;
    public int slotCount
    {
        get { return m_slotCount; }
        set
        {
            for(int i = m_slotCount; i < value; i++)
            {
                InventorySlot tmp = new InventorySlot();
                tmp.onCountChange += () => { onInventoryUpdate?.Invoke(tmp.item.data); };
                slots.Add(tmp);
            }
            m_slotCount = value;
            onSlotCountChange?.Invoke();
        }
    }
    private void Awake()
    {
        slotCount = defaultSlotCount;
    }
    public int Insert(Item item, int count)
    {
        foreach(var i in slots)
        {
            if (i.item == null) continue;
            if(i.item.IsStackable(item) && item.IsStackable(i.item))
            {
                count = i.Insert(item.Copy(), count);
                if (count <= 0) return 0;
            }
        }
        for(int i = 0; i < slotCount; i++)
        {
            count = slots[i].Insert(item.Copy(), count);
            if (count <= 0) return 0;
        }
        onInventoryUpdate?.Invoke(item.data);
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
        onInventoryUpdate?.Invoke(data);
        return true;
    }
    public InventorySaveData Save()
    {
        InventorySaveData data = new();
        for(int i = 0; i < slotCount; i++)
        {
            data.slotSaves.Add(slots[i].Save());
        }
        return data;
    }
    public void Load(InventorySaveData data)
    {
        if(data.slotSaves.Count > slotCount)
        {
            int tmp = slotCount;
            slotCount = data.slotSaves.Count;
            slotCount = tmp;
        }
        for (int i = 0; i < slotCount; i++)
        {
            slots[i].Load(data.slotSaves[i]);
        }
    }
}
[System.Serializable]
public class InventorySaveData
{
    public List<InventorySlotSaveData> slotSaves;
    public InventorySaveData()
    {
        slotSaves = new();
    }
}
