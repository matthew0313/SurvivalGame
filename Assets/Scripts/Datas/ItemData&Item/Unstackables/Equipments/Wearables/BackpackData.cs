using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "Backpack Data", menuName = "Scriptables/Items/Unstackables/Wearables/Backpack", order = 2)]
public class BackpackData : WearableData
{
    [Header("Backpack")]
    [SerializeField] int m_extraSlots;
    public int extraSlots => m_extraSlots;
    public override Item Create() => new Backpack(this);
}
public class Backpack : Wearable
{
    new BackpackData data;
    public Backpack(BackpackData data) : base(data)
    {
        this.data = data;
    }
    public override void OnWear(Player wearer)
    {
        base.OnWear(wearer);
        wearer.inventory.slotCount += data.extraSlots;
    }
    public override void OnUnwear(Player wearer)
    {
        base.OnUnwear(wearer);
        wearer.inventory.slotCount -= data.extraSlots;
    }
}