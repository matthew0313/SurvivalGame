using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bullet : MonoBehaviour
{
    static Dictionary<Bullet, Pooler<Bullet>> pools = new();
    [SerializeField] ItemData m_data;
    [SerializeField] TrailRenderer trail;
    public ItemData data => m_data;
    float damage, speed, range;
    bool isInstance = false;
    Bullet origin = null;
    public void Set(float damage, float speed, float range)
    {
        counter = 0.0f;
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        if(trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
    }
    public Bullet SpawnBullet(Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(this)) pools.Add(this, new Pooler<Bullet>(this));
        Bullet bul = pools[this].GetObject(position, rotation);
        bul.isInstance = true;
        bul.origin = this;
        bul.despawned = false;
        return bul;
    }
    float counter = 0.0f;
    private void Update()
    {
        if (isInstance)
        {
            transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
            counter += Time.deltaTime;
            if(counter >= range / speed)
            {
                Despawn();
            }
        }
    }
    bool despawned = false;
    void Despawn()
    {
        if (despawned) return;
        despawned = true;
        pools[origin].ReleaseObject(this);
        if (trail != null) trail.emitting = false;
    }
}