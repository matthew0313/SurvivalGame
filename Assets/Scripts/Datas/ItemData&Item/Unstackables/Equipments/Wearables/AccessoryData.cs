using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
[CreateAssetMenu(fileName = "Accessory Data", menuName = "Scriptables/Items/Unstackables/Wearables/Accessory", order = 1)]
public class AccessoryData : WearableData
{
    public override Item Create() => new Accessory(this);
}
public class Accessory : Wearable
{
    new AccessoryData data;
    public Accessory(AccessoryData data) : base(data)
    {
        this.data = data;
    }
}