using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "Consumable Data", menuName = "Scriptables/Items/Stackables/Consumable", order = 2)]
public class ConsumableData : StackableItemData
{
    [Header("Consumable")]
    [SerializeField] Consumable m_prefab;
    [SerializeField] int m_maxStack;
    public Consumable prefab => m_prefab;
    public override int maxStack => m_maxStack;
    public override Item Create() => new ConsumableItem(this);
}
public class ConsumableItem : StackableItem
{
    new ConsumableData data;
    public ConsumableItem(ConsumableData data) : base(data)
    {
        this.data = data;
    }
    Consumable instance = null;
    public override void OnWield(Player origin, InventorySlot slot)
    {
        base.OnWield(origin, slot);
        if (instance == null)
        {
            instance = MonoBehaviour.Instantiate(data.prefab, origin.equipAnchor.position, origin.equipAnchor.rotation, origin.equipAnchor);
            instance.Set(this);
        }
        else instance.gameObject.SetActive(true);
        containedSlot.onCountChange += OnHeldCountChange;
    }
    bool descUpdated = false;
    public bool isOnCooldown => !wielder.consumableCooldowns.ContainsKey(data);
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        if (wielder.consumableCooldowns.TryGetValue(data, out _))
        {
            descUpdated = false;
            onDescUpdate?.Invoke();
        }
        else
        {
            if (!descUpdated)
            {
                descUpdated = true;
                onDescUpdate?.Invoke();
            }
            if (InputManager.UseButtonDown() && !isOnCooldown)
            {
                wielder.consumableCooldowns.Add(data, data.prefab.cooldown);
                onDescUpdate?.Invoke();
                instance.OnUse();
                containedSlot.count--;
            }
        }
    }
    public override void OnUnwield()
    {
        base.OnUnwield();
        containedSlot.onCountChange -= OnHeldCountChange;
        instance.gameObject.SetActive(false);
    }
    void OnHeldCountChange() => onDescUpdate?.Invoke();
    public override string DescBar()
    {
        return wielder.consumableCooldowns.TryGetValue(data, out float cooldown) ? Math.Round(cooldown, 1).ToString() : containedSlot.count.ToString();
    }
}