using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : Weapon
{
    new GunItem origin => base.origin as GunItem;
    [Header("Gun")]
    [SerializeField] bool auto = true;
    [SerializeField] float fireRate;
    [SerializeField] float bulletSpeed, bulletRange;
    [SerializeField] Bullet m_bullet;
    [SerializeField] int m_magSize;
    [SerializeField] float reloadTime;
    [SerializeField] Transform firePoint;
    [SerializeField] Animator anim;
    public Bullet bullet => m_bullet;
    public int magSize => m_magSize;
    bool m_reloading = false;
    bool reloading
    {
        get { return m_reloading; }
        set { m_reloading = value; origin.onDescUpdate?.Invoke(); }
    }
    float reloadCounter = 0.0f;
    float counter = 0.0f;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        wielder.inventory.onInventoryUpdate += AmmoCountChangeCheck;
        wielder.rotate = true;
    }
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        if (counter < fireRate) counter += Time.deltaTime;
        if (reloading)
        {
            reloadCounter += Time.deltaTime;
            if(reloadCounter > reloadTime)
            {
                Reload(wielder);
                reloading = false;
            }
        }
        else
        {
            if(SystemInfo.deviceType == DeviceType.Handheld)
            {
                if(origin.mag == 0 && wielder.inventory.Search(bullet.data) > 0)
                {
                    StartReloading();
                }
                else if(auto && wielder.UseButton() || !auto && wielder.UseButtonUp())
                {
                    if (counter >= fireRate && origin.mag > 0)
                    {
                        counter = 0.0f;
                        origin.mag--;
                        Fire(wielder);
                    }
                }
            }
            else if(SystemInfo.deviceType == DeviceType.Desktop)
            {
                if(Input.GetKeyDown(KeyCode.R) && origin.mag < magSize && wielder.inventory.Search(bullet.data) > 0)
                {
                    StartReloading();
                }
                else if(auto && wielder.UseButton() || !auto && wielder.UseButtonDown())
                {
                    if (counter >= fireRate && origin.mag > 0)
                    {
                        counter = 0.0f;
                        origin.mag--;
                        Fire(wielder);
                    }
                }
            }
        }
    }
    void StartReloading()
    {
        if (anim != null) anim.SetTrigger("Reload");
        reloading = true;
        reloadCounter = 0.0f;
    }
    void Reload(Player wielder)
    {
        int reloadAmount = Mathf.Min(magSize - origin.mag, wielder.inventory.Search(bullet.data));
        wielder.inventory.TakeOut(bullet.data, reloadAmount);
        origin.mag += reloadAmount;
        Debug.Log("Reload " + reloadAmount);
    }
    protected virtual void Fire(Player wielder)
    {
        if(anim != null) anim.SetTrigger("Fire");
        Bullet bul = bullet.SpawnBullet(firePoint.position, firePoint.rotation);
        bul.Set(damage, bulletSpeed, bulletRange);
        origin.DurabilityReduce(1.0f);
    }
    public override void OnUnwield(Player wielder)
    {
        reloading = false;
        wielder.inventory.onInventoryUpdate -= AmmoCountChangeCheck;
        wielder.rotate = false;
        base.OnUnwield(wielder);
    }
    void AmmoCountChangeCheck(ItemData item)
    {
        if (item == bullet.data) origin.onDescUpdate?.Invoke();
    }
    public override float DescBarFill()
    {
        return reloading ? 1.0f - reloadCounter / reloadTime : 0.0f;
    }
    public override string DescBar()
    {
        return reloading ? "Reloading..." : $"{origin.mag}/{wielder.inventory.Search(bullet.data)}";
    }
}