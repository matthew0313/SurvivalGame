using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public UIManager()
    {
        Instance = this;
    }
    [Header("Equipment")]
    [SerializeField] GameObject equipDescContents;
    [SerializeField] Text equippedItemName;
    [SerializeField] Text equippedItemDesc;
    [SerializeField] Transform equippedItemDescBar;
    Player player;
    GameObject openTab;
    public bool tabOpen => openTab != null;
    public void OpenTab(GameObject tab)
    {
        if (openTab != null) openTab.SetActive(false);
        openTab = tab;
        if(openTab != null) openTab.SetActive(true);
    }
    public void CloseTab()
    {
        if(openTab != null) openTab.SetActive(false);
        openTab = null;
    }
    [SerializeField] InventorySlotUI itemGrabUI;
    InventorySlot grabbingSlot;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.onEquippedItemChange += EquippedItemChange;
    }
    Item equipDisplaying = null;
    void EquippedItemChange()
    {
        if (equipDisplaying != null) equipDisplaying.onDescUpdate -= EquippedItemDescUpdate;
        equipDisplaying = player.equipped;
        if (equipDisplaying != null)
        {
            equipDescContents.SetActive(true);
            equippedItemName.text = equipDisplaying.data.name;
            equipDisplaying.onDescUpdate += EquippedItemDescUpdate;
            EquippedItemDescUpdate();
        }
        else
        {
            equipDescContents.SetActive(false);
        }
    }
    void EquippedItemDescUpdate()
    {
        equippedItemDesc.text = equipDisplaying.DescBar();
    }
    private void Update()
    {
        if(equipDisplaying != null)
        {
            equippedItemDescBar.localScale = new Vector2(equipDisplaying.DescBarFill(), 1.0f);
        }
        if(SystemInfo.deviceType == DeviceType.Handheld)
        {

        }
    }
}