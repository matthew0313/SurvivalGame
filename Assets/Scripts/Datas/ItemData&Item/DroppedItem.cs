using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class DroppedItem : MonoBehaviour
{
    static Pooler<DroppedItem> m_pool;
    static Pooler<DroppedItem> pool
    {
        get { if (m_pool == null) m_pool = new Pooler<DroppedItem>(Resources.Load<DroppedItem>("DroppedItem")); return m_pool; }
    }
    Item m_item = null;
    int m_count = 0;
    public Item item
    {
        get { return m_item; }
        private set
        {
            m_item = value;
            if(m_item == null)
            {
                pool.ReleaseObject(this);
            }
        }
    }
    public int count
    {
        get { return m_count; }
        set
        {
            m_count = value;
            if (m_count <= 0) item = null;
        }
    }
    [SerializeField] SpriteRenderer itemImage;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
        rb.freezeRotation = true;
        rb.mass = 0;
    }
    private void OnEnable()
    {
        DroppedItemManager.Instance.Add(this);
    }
    private void OnDisable()
    {
        DroppedItemManager.Instance.Remove(this);
    }
    public void Set(Item item, int count)
    {
        itemImage.sprite = item.data.image;
        this.item = item;
        this.count = count;
    }
    public bool canPick => rb.velocity.magnitude < 0.5f;
    public void SetVelocity(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
    public static DroppedItem Create(Vector2 position) => pool.GetObject(position, Quaternion.identity);
    public DroppedItemSaveData Save()
    {
        DroppedItemSaveData tmp = new();
        tmp.position = transform.position;
        tmp.velocity = rb.velocity;
        tmp.item = item.data;
        item.Save(tmp.data);
        tmp.count = count;
        return tmp;
    }
    public void Load(DroppedItemSaveData data)
    {
        rb.velocity = data.velocity;
        Item tmp = data.item.Create();
        tmp.Load(data.data);
        Set(tmp, data.count);
    }
}
[System.Serializable]
public class DroppedItemSaveData
{
    public Vector2 position, velocity;
    public ItemData item;
    public DataUnit data;
    public int count;
    public DroppedItemSaveData()
    {
        data = new DataUnit();
    }
}