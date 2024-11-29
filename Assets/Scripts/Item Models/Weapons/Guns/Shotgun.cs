using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Shotgun : Gun
{
    [Header("Shotgun")]
    [SerializeField] protected int fireCount;

    protected override void FireBullet()
    {
        for (int i = 0; i < fireCount; i++) base.FireBullet();
    }
}