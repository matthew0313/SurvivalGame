using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bullet : MonoBehaviour
{
    Pooler<Bullet> pool;
    [SerializeField] GameObject body;
    [SerializeField] TrailRenderer trail;
    HpChangeData damage;
    float speed, range;
    int pierce = 1;
    bool isInstance = false;

    Rigidbody2D rb;
    public void Set(HpChangeData damage, float speed, float range, int pierce = 1)
    {
        counter = 0.0f;
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        this.pierce = pierce;
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
    }
    public Bullet SpawnBullet(Vector3 position, Quaternion rotation)
    {
        if (isInstance) return null;
        if (pool == null) pool = new Pooler<Bullet>(this, 100, 0, OnTakeout, OnRelease);
        Bullet bul = pool.GetObject(position, rotation);
        bul.isInstance = true;
        bul.pool = pool;
        bul.despawned = false;
        return bul;
    }
    void OnTakeout(Bullet self)
    {
        self.body.SetActive(true);
        self.enabled = true;
    }
    void OnRelease(Bullet self)
    {
        self.body.SetActive(false);
        self.enabled = false;
    }
    float counter = 0.0f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (isInstance)
        {
            transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
            counter += Time.deltaTime;
            if (counter >= range / speed)
            {
                Despawn();
            }
        }
    }
    bool despawned = false;
    void Despawn()
    {
        if (!isInstance || despawned) return;
        despawned = true;
        pool.ReleaseObject(this);
        if (trail != null) trail.emitting = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HpComp tmp = collision.GetComponent<HpComp>();
        if (tmp != null)
        {
            tmp.GetDamage(damage);
            if (--pierce <= 0) Despawn();
        }
        else Despawn();
    }
}