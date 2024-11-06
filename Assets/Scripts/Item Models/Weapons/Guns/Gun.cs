using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : Weapon
{
    new GunItem origin => base.origin as GunItem;
    [Header("Gun")]
    [SerializeField] protected bool auto = true;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float bulletSpeed, bulletRange, bulletSpread;
    [SerializeField] protected Bullet bullet;
    [SerializeField] protected ItemData m_bulletItem;
    [SerializeField] protected int m_magSize;
    [SerializeField] protected float reloadTime;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected Animator anim;
    public ItemData bulletItem => m_bulletItem;
    public int magSize => m_magSize;
    bool m_reloading = false;
    public bool reloading
    {
        get { return m_reloading; }
        set { m_reloading = value; origin.onDescUpdate?.Invoke(); }
    }
    float reloadCounter = 0.0f;
    float counter = 0.0f;
    public override void OnWield()
    {
        base.OnWield();
        wielder.inventory.onInventoryUpdate += AmmoCountChangeCheck;
        wielder.rotate = true;
    }
    public override void OnWieldUpdate()
    {
        base.OnWieldUpdate();
        if (counter < fireRate) counter += Time.deltaTime;
        if (reloading)
        {
            reloadCounter += Time.deltaTime;
            if(reloadCounter > reloadTime)
            {
                Reload();
                reloading = false;
            }
        }
        else
        {
            if (auto && InputManager.UseButton() || !auto && InputManager.UseButtonDown())
            {
                if (counter >= fireRate && origin.mag > 0)
                {
                    counter = 0.0f;
                    origin.mag--;
                    Fire();
                }
            }
            else if ((InputManager.ReloadButtonDown() || SystemInfo.deviceType == DeviceType.Handheld && origin.mag <= 0) && origin.mag < magSize && wielder.inventory.Search(bulletItem) > 0)
            {
                if (anim != null) anim.SetTrigger("Reload");
                reloading = true;
                reloadCounter = 0.0f;
            }
        }
    }
    void Reload()
    {
        int reloadAmount = Mathf.Min(magSize - origin.mag, wielder.inventory.Search(bulletItem));
        wielder.inventory.TakeOut(bulletItem, reloadAmount);
        origin.mag += reloadAmount;
        Debug.Log("Reload " + reloadAmount);
    }
    void Fire()
    {
        if(anim != null) anim.SetTrigger("Fire");
        FireBullet();
        origin.DurabilityReduce(1.0f);
    }
    protected virtual void FireBullet()
    {
        Bullet bul = bullet.SpawnBullet(firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-bulletSpread, bulletSpread)));
        bul.Set(damage, bulletSpeed, bulletRange);
    }
    public override void OnUnwield()
    {
        reloading = false;
        wielder.inventory.onInventoryUpdate -= AmmoCountChangeCheck;
        wielder.rotate = false;
        base.OnUnwield();
    }
    void AmmoCountChangeCheck(ItemData item)
    {
        if (item == bulletItem) origin.onDescUpdate?.Invoke();
    }
    public override float DescBarFill()
    {
        return reloading ? 1.0f - reloadCounter / reloadTime : 0.0f;
    }
    public override string DescBar()
    {
        return reloading ? "Reloading..." : $"{origin.mag}/{wielder.inventory.Search(bulletItem)}";
    }
}