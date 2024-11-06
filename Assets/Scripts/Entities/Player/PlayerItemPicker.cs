using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerItemPicker : MonoBehaviour
{
    Player player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("DroppedItem"))
        {
            DroppedItem tmp = collision.GetComponent<DroppedItem>();
            if (tmp.canPick == false) return;
            int prev = tmp.count;
            if (tmp.item != null) tmp.count = player.inventory.Insert(tmp.item, tmp.count);
            else tmp.count = 0;
        }
    }
}