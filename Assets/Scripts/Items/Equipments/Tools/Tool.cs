using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ToolData : EquipmentData
{

}
public abstract class Tool : Equipment
{
    new ToolData data;
    public Tool(ToolData data) : base(data)
    {
        this.data = data;
    }
}