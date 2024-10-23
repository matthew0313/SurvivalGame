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
    void ItemChange()
    {
        if (checkingSlot.item != null)
        {
            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = checkingSlot.item.data.image;
        }
        else itemIcon.gameObject.SetActive(false);
    }
    void CountChange()
    {
        if (checkingSlot.count > 1)
        {
            itemCount.gameObject.SetActive(true);
            itemCount.text = $"x{checkingSlot.count}";
        }
        else itemCount.gameObject.SetActive(false);
    }
}
