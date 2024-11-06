using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroppedItemManager : MonoBehaviour, ISavable
{
    public static DroppedItemManager Instance { get; private set; }
    public DroppedItemManager()
    {
        Instance = this;
    }
    List<DroppedItem> droppedItems = new();
    public void Add(DroppedItem item) => droppedItems.Add(item);
    public void Remove(DroppedItem item) => droppedItems.Remove(item);

    public void Save(SaveData data)
    {
        foreach (var i in droppedItems) data.droppedItems.Add(i.Save());
    }

    public void Load(SaveData data)
    {
        foreach(var i in data.droppedItems)
        {
            if (i.item == null) continue;
            DroppedItem tmp = DroppedItem.Create(i.position);
            tmp.Load(i);
        }
    }
}