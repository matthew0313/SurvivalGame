using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Layer<T> : State<T>
{
    protected Dictionary<string, State<T>> states = new();
    protected Dictionary<State<T>, string> stateNames = new();
    protected State<T> currentState = null;
    protected State<T> defaultState = null;
    public Layer(T origin, Layer<T> parent) : base(origin, parent)
    {

    }
    protected void AddState(string name, State<T> state)
    {
        states[name] = state;
        stateNames[state] = name;
    }
    public string GetStateName(State<T> state)
    {
        if (!stateNames.ContainsKey(state))
        {
            Debug.LogError("State not found. Make sure you're using AddState() to add states.");
            return null;
        }
        else return stateNames[state];
    }
    public void ChangeState(params string[] route)
    {
        List<string> stateRoute = new List<string>(route);
        Enter(stateRoute);
    }
    public override void Enter(List<string> stateRoute)
    {
        State<T> nextState;
        if(stateRoute.Count == 0)
        {
            nextState = GetDefaultState();
        }
        else
        {
            if (!states.ContainsKey(stateRoute[0]))
            {
                Debug.LogError("Attempted to switch to a state that does not exist, switching to default state");
                nextState = GetDefaultState();
            }
            else
            {
                nextState = states[stateRoute[0]];
            }
            stateRoute.RemoveAt(0);
        }
        if(currentState != null && currentState != nextState)
        {
            currentState.OnStateExit();
        }
        if(currentState != nextState)
        {
            currentState = nextState;
            currentState.OnStateEnter();
        }
        else if(currentState == nextState)
        {
            currentState.OnStateReEnter();
        }
        currentState.Enter(stateRoute);
    }
    protected virtual State<T> GetDefaultState()
    {
        return defaultState;
    }
    public override void OnStateExit()
    {
        base.OnStateExit();
        currentState.OnStateExit();
    }
    public override void OnStateUpdate()
    {
        base.OnStateUpdate();
        currentState.OnStateUpdate();
    }
    public override void OnStateFixedUpdate()
    {
        base.OnStateFixedUpdate();
        currentState.OnStateFixedUpdate();
    }
    public override string GetFSMPath()
    {
        return $"{base.GetFSMPath()}->{currentState.GetFSMPath()}";
    }
}
