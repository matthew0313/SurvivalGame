using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Medkit : Consumable
{
    [Header("Medkit")]
    [SerializeField] float healAmount;
    public override void OnUse()
    {
        base.OnUse();
        wielder.hp.Heal(healAmount);
    }
}