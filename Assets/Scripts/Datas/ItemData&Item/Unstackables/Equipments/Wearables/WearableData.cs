using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WearableData : UnstackableItemData
{
    [Header("Wearable")]
    [SerializeField] WearableTraits m_traits;
    [SerializeField] float m_extraHealth;
    [SerializeField] float m_extraSpeed;
    public WearableTraits traits => m_traits;
    public float extraHealth => m_extraHealth;
    public float extraSpeed => m_extraSpeed;
}
public abstract class Wearable : UnstackableItem
{
    new WearableData data;
    public Wearable(WearableData data) : base(data)
    {
        this.data = data;
    }
    public virtual void OnWear(Player wearer)
    {
        if ((data.traits & WearableTraits.ExtraHp) > 0) wearer.hp.bonusHp += data.extraHealth;
        if ((data.traits & WearableTraits.ExtraSpeed) > 0) wearer.bonusMoveSpeed += data.extraSpeed;
    }
    public virtual void OnUnwear(Player wearer)
    {
        if ((data.traits & WearableTraits.ExtraHp) > 0) wearer.hp.bonusHp -= data.extraHealth;
        if ((data.traits & WearableTraits.ExtraSpeed) > 0) wearer.bonusMoveSpeed -= data.extraSpeed;
    }
}
[System.Serializable]
[Flags]
public enum WearableTraits
{
    ExtraHp = 1<<0,
    ExtraSpeed = 1<<1
}