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
    protected Player wielder => origin.wielder;
    public virtual void OnWield() { }
    public virtual void OnWieldUpdate() { }
    public virtual void OnUnwield() { }
    public virtual float DescBarFill() => 0.0f;
    public virtual string DescBar() => "";
}