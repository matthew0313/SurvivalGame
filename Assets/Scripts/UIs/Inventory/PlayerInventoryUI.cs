using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInventoryUI : MonoBehaviour
{
    [Header("InventoryUI")]
    [SerializeField] Transform slotAnchor;
    [SerializeField] InventorySlotUI clothing, accessory1, accessory2, backpack;

    [SerializeField] InventorySlotUI slotPrefab;

    List<InventorySlotUI> slots = new();
    int displaying = 0;
    Player player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.inventory.onSlotCountChange += OnSlotCountChange;
        clothing.Set(player.clothingSlot);
        accessory1.Set(player.accessorySlot1);
        accessory2.Set(player.accessorySlot2);
        backpack.Set(player.backpackSlot);
    }
    private void Start()
    {
        OnSlotCountChange();
    }
    void OnSlotCountChange()
    {
        for(int i = 6+slots.Count; i < player.inventory.slotCount; i++)
        {
            InventorySlotUI tmp = Instantiate(slotPrefab, slotAnchor);
            tmp.Set(player.inventory.slots[i]);
            slots.Add(tmp);
        }
        for(int i = 0; i < player.inventory.slotCount-6; i++)
        {
            slots[i].gameObject.SetActive(true);
        }
        for(int i = player.inventory.slotCount-6; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
    }
}