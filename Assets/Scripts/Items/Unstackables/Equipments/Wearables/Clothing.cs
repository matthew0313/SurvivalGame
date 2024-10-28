using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClothingData : WearableData
{
    [Header("Clothing")]
    [SerializeField] float m_extraHealth;
    public float extraHealth => m_extraHealth;
    public override Item Create() => new Clothing(this);
}
public class Clothing : Wearable
{
    new ClothingData data;
    public Clothing(ClothingData data) : base(data)
    {
        this.data = data;
    }
    public override void OnWear(Player wearer)
    {
        base.OnWear(wearer);
        
    }
}