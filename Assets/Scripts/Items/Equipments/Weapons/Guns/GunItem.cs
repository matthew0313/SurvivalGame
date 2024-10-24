using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Gun Data", menuName = "Scriptables/Items/Equipment/Weapons/Gun", order = 1)]
public class GunData : WeaponData
{
    public override Item Create() => new GunItem(this);
}
public class GunItem : WeaponItem
{
    new GunData data;
    public GunItem(GunData data) : base(data)
    {
        this.data = data;
    }
    int m_mag = 0;
    public int mag
    {
        get { return m_mag; }
        set { m_mag = value; onDescUpdate?.Invoke(); }
    }
}