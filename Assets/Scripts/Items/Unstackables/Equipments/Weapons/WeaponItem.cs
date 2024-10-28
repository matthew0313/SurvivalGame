using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Scriptables/Items/Unstackables/Equipment/Weapons/Weapon", order = 0)]
public class WeaponData : EquipmentData 
{
    [Header("Weapon")]
    [SerializeField] Weapon m_prefab;
    public Weapon prefab => m_prefab;
    public override Item Create() => new WeaponItem(this);
}
public class WeaponItem : Equipment
{
    new WeaponData data;
    public WeaponItem(WeaponData data) : base(data)
    {
        this.data = data;
    }
    Weapon instance = null;
    public override void OnWield(Player origin, InventorySlot slot)
    {
        base.OnWield(origin, slot);
        if (instance == null)
        {
            instance = MonoBehaviour.Instantiate(data.prefab, origin.equipAnchor.position, origin.equipAnchor.rotation, origin.equipAnchor);
            instance.Set(this);
        }
        else instance.gameObject.SetActive(true);
        instance.OnWield(origin);
    }
    public override void OnWieldUpdate(Player origin)
    {
        base.OnWieldUpdate(origin);
        instance.OnWieldUpdate(origin);
    }
    public override void OnUnwield(Player origin)
    {
        instance.OnUnwield(origin);
        instance.gameObject.SetActive(false);
        base.OnUnwield(origin);
    }
    public override float DescBarFill()
    {
        return instance.DescBarFill();
    }
    public override string DescBar()
    {
        return instance.DescBar();
    }
}