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
    public override void OnWield(Player wielder, InventorySlot slot)
    {
        base.OnWield(wielder, slot);
        if (instance == null)
        {
            instance = MonoBehaviour.Instantiate(data.prefab, wielder.equipAnchor.position, wielder.equipAnchor.rotation, wielder.equipAnchor);
            instance.Set(this);
        }
        else instance.gameObject.SetActive(true);
        instance.OnWield();
    }
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        instance.OnWieldUpdate();
    }
    public override void OnUnwield()
    {
        instance.OnUnwield();
        instance.gameObject.SetActive(false);
        base.OnUnwield();
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