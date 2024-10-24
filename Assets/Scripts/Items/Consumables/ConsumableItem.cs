using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Consumable Data", menuName = "Scriptables/Items/Consumable", order = 2)]
public class ConsumableData : ItemData
{
    [Header("Consumable")]
    [SerializeField] Consumable m_prefab;
    [SerializeField] int m_maxStack;
    public Consumable prefab => m_prefab;
    public override int maxStack => m_maxStack;
}
public class ConsumableItem : Item
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
        instance.OnWield(origin);
    }
    public override void OnWieldUpdate(Player origin)
    {
        base.OnWieldUpdate(origin);
        instance.OnWieldUpdate(origin);
    }
    public override void OnUnwield(Player origin)
    {
        base.OnUnwield(origin);
        instance.OnUnwield(origin);
        instance.gameObject.SetActive(false);
    }
}
