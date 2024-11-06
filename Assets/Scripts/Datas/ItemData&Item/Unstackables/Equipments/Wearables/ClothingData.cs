using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "Clothing Data", menuName = "Scriptables/Items/Unstackables/Wearables/Clothing", order = 0)]
public class ClothingData : WearableData
{
    public override Item Create() => new Clothing(this);
}
public class Clothing : Wearable
{
    new ClothingData data;
    public Clothing(ClothingData data) : base(data)
    {
        this.data = data;
    }
}