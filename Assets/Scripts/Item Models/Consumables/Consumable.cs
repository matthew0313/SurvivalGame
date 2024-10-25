using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
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
    public virtual void OnWield(Player wielder) { }
    public virtual void OnWieldUpdate(Player wielder) { }
    public virtual void OnUnwield(Player wielder) { }
}