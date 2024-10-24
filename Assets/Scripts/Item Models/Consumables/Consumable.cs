using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Consumable : MonoBehaviour
{
    protected ConsumableItem origin { get; private set; }
    public void Set(ConsumableItem origin)
    {
        this.origin = origin;
    }
    public virtual void OnWield(Player wielder) { }
    public virtual void OnWieldUpdate(Player wielder) { }
    public virtual void OnUnwield(Player wielder) { }
    public virtual void OnConsume(Player consumer) { }
}