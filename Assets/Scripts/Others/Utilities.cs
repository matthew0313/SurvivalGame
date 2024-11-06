using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[System.Serializable]
public struct ItemDataIntPair
{
    public ItemData item;
    public int count;
}
public struct ItemIntPair
{
    public Item item;
    public int count;
}
[System.Serializable]
public struct TimeVal
{
    [SerializeField] int hours;
    [SerializeField] int minutes;
    [SerializeField] int seconds;
    public float time => hours * 3600 + minutes * 60 + seconds;
}
public static class Utilities
{
    public static Vector2 RandomAngle(float start, float end)
    {
        float angle = Random.Range(start, end);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}