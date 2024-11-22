using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public Transform weaponPivot;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireCooldown = 0.5f;

    private float lastFireTime = 0f; // 마지막 공격 시간 기록\
    private Transform target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        if (target != null)
        {
            AimWeapon();
        }

        if (Time.time - lastFireTime >= fireCooldown)
        {
            Fire();
        }
    }

    void AimWeapon()
    {
        Vector2 direction = target.position - weaponPivot.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Fire()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = firePoint.right * projectileSpeed;
            }
            lastFireTime = Time.time;
        }
    }
}
