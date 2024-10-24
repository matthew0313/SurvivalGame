using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemCount;
    [SerializeField] GameObject durabilityBar; 
    [SerializeField] Transform durabilityScaler;
    InventorySlot checkingSlot;
    public void Set(InventorySlot slot)
    {
        if(checkingSlot != null)
        {
            checkingSlot.onItemChange -= ItemChange;
            checkingSlot.onCountChange -= CountChange;
        }
        checkingSlot = slot;
        checkingSlot.onItemChange += ItemChange;
        checkingSlot.onCountChange += CountChange;
        ItemChange();
        CountChange();
    }
    Item checkingItem = null;
    void ItemChange()
    {
        if(checkingItem != null)
        {
            if(checkingItem is Equipment)
            {
                (checkingItem as Equipment).onDurabilityChange -= DurabilityBarUpdate;
            }
        }
        checkingItem = checkingSlot.item;
        if (checkingItem != null)
        {
            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = checkingItem.data.image;
        }
        else
        {
            itemIcon.gameObject.SetActive(false);
        }
        if (checkingItem is Equipment)
        {
            durabilityBar.SetActive(true);
            (checkingItem as Equipment).onDurabilityChange += DurabilityBarUpdate;
            DurabilityBarUpdate();
        }
        else
        {
            durabilityBar.SetActive(false);
        }
    }
    void CountChange()
    {
        if (checkingSlot.count > 1)
        {
            itemCount.gameObject.SetActive(true);
            itemCount.text = $"{checkingSlot.count}";
        }
        else itemCount.gameObject.SetActive(false);
    }
    void DurabilityBarUpdate()
    {
        durabilityScaler.localScale = new Vector2((checkingItem as Equipment).durability / (checkingItem as Equipment).maxDurability, 1.0f);
    }
}
