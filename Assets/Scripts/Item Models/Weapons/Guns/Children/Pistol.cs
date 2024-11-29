using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pistol : Gun
{
    [SerializeField] protected float bleedDamage, bleedDuration;
    protected override Debuff inflictedDebuff => new Bleeding(bleedDuration, bleedDamage);
}