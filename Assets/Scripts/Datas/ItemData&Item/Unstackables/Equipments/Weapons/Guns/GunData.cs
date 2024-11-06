using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Gun Data", menuName = "Scriptables/Items/Unstackables/Equipment/Weapons/Gun", order = 1)]
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
        m_mag = (data.prefab as Gun).magSize;
    }
    int m_mag = 0;
    public int mag
    {
        get { return m_mag; }
        set { m_mag = value; onDescUpdate?.Invoke(); }
    }
    public override Item Copy()
    {
        GunItem item = base.Copy() as GunItem;
        item.mag = mag;
        return item;
    }
    protected override void OnBreak()
    {
        base.OnBreak();
        wielder.AddItem_DropRest((data.prefab as Gun).bulletItem.Create(), mag);
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.integers.Add("Mag", mag);
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        if (data.integers.TryGetValue("Mag", out int tmp)) mag = tmp;
    }
}