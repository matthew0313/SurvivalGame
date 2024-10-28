using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class WearableData : EquipmentData
{

}
public abstract class Wearable : Equipment
{
    new WearableData data;
    public Wearable(WearableData data) : base(data)
    {
        this.data = data;
    }
    public virtual void OnWear(Player wearer)
    {

    }
    public virtual void OnUnwear(Player wearer)
    {

    }
}