using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T>
{
    protected readonly T origin;
    protected readonly Layer<T> parentLayer;
    protected virtual TopLayer<T> root => parentLayer.root;
    public State(T origin, Layer<T> parent)
    {
        this.origin = origin;
        this.parentLayer = parent;
    }
    public virtual void Enter(List<string> stateRoute)
    {
        if(stateRoute.Count > 0)
        {
            Debug.LogWarning("Excessive state route found.");
        }
        AlertStateChange();
    }
    public virtual void OnStateEnter()
    {

    }
    public virtual void OnStateReEnter()
    {

    }
    public virtual void OnStateUpdate()
    {
        
    }
    public virtual void OnStateFixedUpdate()
    {

    }
    public virtual void OnStateExit()
    {

    }
    public virtual void AlertStateChange() => parentLayer.AlertStateChange();
    public virtual string GetFSMPath()
    {
        return parentLayer.GetStateName(this);
    }
}
