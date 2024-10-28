using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Weapon : MonoBehaviour
{
    protected WeaponItem origin { get; private set; }
    public virtual void Set(WeaponItem origin)
    {
        this.origin = origin;
    }
    [Header("Weapon")]
    [SerializeField] float m_damage;
    public float damage => m_damage;
    protected Player wielder { get; private set; } = null;
    public virtual void OnWield(Player wielder)
    {
        this.wielder = wielder;
    }
    public virtual void OnWieldUpdate(Player wielder) { }
    public virtual void OnUnwield(Player wielder)
    {
        this.wielder = null;
    }
    public virtual float DescBarFill() => 0.0f;
    public virtual string DescBar() => "";
}