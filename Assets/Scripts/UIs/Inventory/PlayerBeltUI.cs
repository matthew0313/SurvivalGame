using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerBeltUI : MonoBehaviour
{
    [SerializeField]
    InventorySlotUI[] slots = new InventorySlotUI[6];
    [SerializeField] RectTransform equipIndicator;
    Player player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        for(int i = 0; i < 6; i++)
        {
            slots[i].Set(player.inventory.slots[i]);
        }
    }
    private void Update()
    {
        EquipSlotChange();
    }
    public void TrySlotChange(int slot)
    {
        if (UIManager.Instance.isTabOpen) return;
        player.SwapSlot(slot);
    }
    void EquipSlotChange()
    {
        equipIndicator.position = slots[player.equippedIndex].transform.position;
    }
}