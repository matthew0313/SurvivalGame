using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class TopLayer<T> : Layer<T>
{
    public Action onFSMChange;
    protected override TopLayer<T> root => this;
    public TopLayer(T origin) : base(origin, null)
    {
       
    }
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        ChangeState();
    }
    public override void AlertStateChange() => onFSMChange?.Invoke();
    public override string GetFSMPath()
    {
        return $"Top->{currentState.GetFSMPath()}";
    }
}
