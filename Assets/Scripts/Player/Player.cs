using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Apperance")]
    [SerializeField] Transform model;

    [Header("Input")]
    [SerializeField] InputActionReference moveInput;
    [SerializeField] InputActionReference aimInput;
    [SerializeField] UtilityButton useButton;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    public bool canMove = true;
    public bool rotate = false;

    [Header("Components")]
    [SerializeField] Animator anim;
    Rigidbody2D rb;

    [Header("Inventory")]
    [SerializeField] ItemIntPair[] startContents = new ItemIntPair[36];
    [SerializeField] Transform m_equipAnchor, rotator;
    public Transform equipAnchor => m_equipAnchor;
    public readonly Inventory inventory = new Inventory(36);
    public readonly Dictionary<ConsumableData, float> consumableCooldowns = new();

    //anim cache
    readonly int moveID = Animator.StringToHash("Moving");
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        for (int i = 0; i < 36; i++)
        {
            if (startContents[i].item == null) continue;
            inventory.slots[i].SetItem(startContents[i].item.Create());
            inventory.slots[i].count = startContents[i].count;
        }
    }
    private void Start()
    {
        equippedSlot.onItemChange += OnEquippedSlotUpdate;
        OnEquippedSlotUpdate();
    }
    private void Update()
    {
        EquipUpdate();
    }
    private void FixedUpdate()
    {
        Move();
        Rotate();
    }
    void Move()
    {
        if (!canMove) return;
        Vector2 move = moveInput.action.ReadValue<Vector2>();
        rb.MovePosition(rb.position + move * moveSpeed * Time.deltaTime);
        anim.SetBool(moveID, move != Vector2.zero);
        if (move.x < 0)
        {
            model.localScale = new Vector2(-1.0f, 1.0f);
        }
        else if (move.x > 0)
        {
            model.localScale = new Vector2(1.0f, 1.0f);
        }
    }
    void Rotate()
    {
        if (!rotate)
        {
            rotator.rotation = Quaternion.identity;
            equipAnchor.localScale = Vector2.one;
            rotator.localScale = Vector2.one;
        }
        else
        {
            rotator.localScale = model.localScale;
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                rotator.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(mousePos.y - rotator.position.y, mousePos.x - rotator.position.x) * Mathf.Rad2Deg);
                if(mousePos.x > rotator.position.x)
                {
                    equipAnchor.localScale = new Vector2(1.0f, 1.0f);
                }
                else if(mousePos.x < rotator.position.x)
                {
                    equipAnchor.localScale = new Vector2(1.0f, -1.0f);
                }
            }
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                if (useButton.mode == PressedMode.Pressed || useButton.mode == PressedMode.Down)
                {
                    Vector2 aim = aimInput.action.ReadValue<Vector2>();
                    rotator.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);
                    if(aim.x > 0)
                    {
                        equipAnchor.localScale = new Vector2(1.0f, 1.0f);
                    }
                    else if(aim.x < 0)
                    {
                        equipAnchor.localScale = new Vector2(1.0f, -1.0f);
                    }
                }
            }
        }
    }
    public Action onEquippedItemChange;
    public int equippedIndex { get; private set; } = 0;
    InventorySlot equippedSlot => inventory.slots[equippedIndex];
    public Item equipped { get; private set; } = null;
    public void SwapSlot(int index)
    {
        if (index > 6 || index < 0 || index == equippedIndex) return;
        equippedSlot.onItemChange -= OnEquippedSlotUpdate;
        equippedIndex = index;
        equippedSlot.onItemChange += OnEquippedSlotUpdate;
        OnEquippedSlotUpdate();
    }
    void OnEquippedSlotUpdate()
    {
        if (equipped != null) equipped.OnUnwield(this);
        equipped = equippedSlot.item;
        if (equipped != null) equipped.OnWield(this, equippedSlot);
        onEquippedItemChange?.Invoke();
    }
    void EquipUpdate()
    {
        if(SystemInfo.deviceType == DeviceType.Desktop)
        {
            for(int i = 0; i < 6; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SwapSlot(i);
                    break;
                }
            }
        }
        if (equipped != null) equipped.OnWieldUpdate(this);
    }
    List<ConsumableData> removeQueue = new();
    void CooldownUpdate()
    {
        foreach(var i in consumableCooldowns.Keys)
        {
            consumableCooldowns[i] -= Time.deltaTime;
            if (consumableCooldowns[i] <= 0)
            {
                removeQueue.Add(i);
            }
        }
        foreach(var i in removeQueue)
        {
            consumableCooldowns.Remove(i);
        }
        removeQueue.Clear();
    }
    public bool UseButtonDown()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            return useButton.IsButtonDown();
        }
        return false;
    }
    public bool UseButton()
    {
        if(SystemInfo.deviceType == DeviceType.Handheld)
        {
            return useButton.IsButton();
        }
        return false;
    }
    public bool UseButtonUp()
    {
        if(SystemInfo.deviceType == DeviceType.Handheld)
        {
            return useButton.IsButtonUp();
        }
        return false;
    }
}
