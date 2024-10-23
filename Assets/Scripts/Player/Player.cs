using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] InputActionReference input;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    public bool canMove = true;
    public Vector2 lastMovedDirection { get; private set; } = Vector2.right;

    [Header("Components")]
    [SerializeField] SpriteRenderer model;
    Rigidbody rb;

    [Header("Inventory")]
    [SerializeField] ItemIntPair[] startContents = new ItemIntPair[36];
    [SerializeField] SpriteRenderer equippedGear;
    Inventory inventory = new Inventory(36);

    public int miningLevel { get; private set; } = 0;
    public float miningExpReq => miningLevel * 100 + 100;
    public float miningExp { get; private set; } = 0;
    public int choppingLevel { get; private set; } = 0;
    public float choppingExpReq => miningLevel * 100 + 100;
    public float choppingExp { get; private set; } = 0;
    public int combatLevel { get; private set; } = 0;
    public float combatExpReq => miningLevel * 100 + 100;
    public float combatExp { get; private set; } = 0;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        for (int i = 0; i < 36; i++)
        {
            inventory.slots[i].SetItem(startContents[i].item.Create());
            inventory.slots[i].count = startContents[i].count;
        }
    }
    private void Update()
    {
        EquipUpdate();
    }
    private void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        if (!canMove) return;
        Vector2 move = input.action.ReadValue<Vector2>();
        rb.MovePosition(rb.position + (Vector3)move * moveSpeed * Time.deltaTime);
        lastMovedDirection = move;
        if (move.x < 0) model.flipX = true;
        else if (move.x > 0) model.flipX = false;
    }
    int equippedIndex = 0;
    InventorySlot equippedSlot => inventory.slots[equippedIndex];
    Item equipped = null;
    public void SwapSlot(int index)
    {
        if (index > 6 || index < 0 || index == equippedIndex) return;
        EndEquip();
        equippedIndex = index;
        StartEquip();
    }
    void EndEquip()
    {
        if(equipped is StructureItem)
        {
            //delete the structure example
        }
        equipped = null;
    }
    void EquipUpdate()
    {
        if(equipped == null || equipped is Tool)
        {
            //hover mouse over attackable objects to mark them
        }
        else if(equipped is StructureItem)
        {
            //move the structure example around
        }
    }
    public void EquipUse()
    {
        if (equipped == null || equipped is Tool)
        {
            //attack the marked attackable object
        }
        else if(equipped is StructureItem)
        {
            //place the structure prefab
        }
    }
    void StartEquip()
    {
        equipped = equippedSlot.item;
        if(equipped == null)
        {
            equippedGear.sprite = null;
        }
        else
        {
            equippedGear.sprite = equipped.data.image;
        }
        if(equipped is StructureItem)
        {
            //instantiate the structure example
        }
    }
}
