using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptables/Items/Bullet", order = 0)]
public class BulletData : ItemData
{
    public override int maxStack => 999;
}