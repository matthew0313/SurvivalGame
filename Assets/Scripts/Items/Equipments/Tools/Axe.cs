using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeData : ToolData
{
    public override Item Create() => new Axe(this);
}
public class Axe : Tool
{
    new AxeData data;
    public Axe(AxeData data) : base(data)
    {
        this.data = data;
    }
}