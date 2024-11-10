using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Consumable : MonoBehaviour
{
    protected ConsumableItem origin { get; private set; }
    public void Set(ConsumableItem origin)
    {
        this.origin = origin;
    }
    [Header("Consumable")]
    [SerializeField] float m_cooldown;
    public float cooldown => m_cooldown;
    public Player wielder => origin.wielder;
    public virtual void OnWield() { }
    public virtual void OnUse() { }
    public virtual void OnUnwield() { }
    public virtual string DescBar() => "Ready";
    public virtual float DescBarFill() => 0.0f;
}