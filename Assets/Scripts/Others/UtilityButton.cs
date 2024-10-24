using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UtilityButton : Button
{
    public PressedMode mode { get; private set; } = PressedMode.None;
    private void Update()
    {
        if (IsPressed())
        {
            if (mode == PressedMode.None)
            {
                mode = PressedMode.Down;
            }
            else if (mode == PressedMode.Down)
            {
                mode = PressedMode.Pressed;
            }
        }
        else
        {
            if (mode == PressedMode.Down || mode == PressedMode.Pressed)
            {
                mode = PressedMode.Up;
            }
            else if(mode == PressedMode.Up)
            {
                mode = PressedMode.None;
            }
        }
    }
    public bool IsButtonDown() => mode == PressedMode.Down;
    public bool IsButton() => mode == PressedMode.Pressed || mode == PressedMode.Down;
    public bool IsButtonUp() => mode == PressedMode.Up;
}
public enum PressedMode
{
    None,
    Down,
    Pressed,
    Up
}
