using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StructureItemData : ItemData
{
    [Header("Structure")]
    [SerializeField] Structure m_prefab;
    public Structure prefab => m_prefab;
    public override Item Create() => new StructureItem(this);
}
public class StructureItem : Item
{
    new StructureItemData data;
    public StructureItem(StructureItemData data) : base(data)
    {
        this.data = data;
    }
}