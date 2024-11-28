using System;
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
        float angle = UnityEngine.Random.Range(start, end);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    public static void DebugCircle(Vector2 center, float radius, Color color, float duration)
    {
        for(int i = 0; i < 360; i++)
        {
            Debug.DrawLine(center + new Vector2(Mathf.Cos(i), Mathf.Sin(i)) * radius, center + new Vector2(Mathf.Cos(i+1), Mathf.Sin(i+1)) * radius, color, duration);
        }
    }
    public static IEnumerator MoveTo(this Transform target, Vector2 position, float speed, Action onArrival = null)
    {
        while(true)
        {
            Vector2 move = (position - (Vector2)target.position).normalized * Time.deltaTime * speed;
            if (move.magnitude >= Vector2.Distance(position, target.position))
            {
                target.position = position;
                break;
            }
            else target.Translate(move);
            yield return null;
        }
        onArrival?.Invoke();
    }
    public static string TimeCode(int seconds)
    {
        int hours = seconds / 3600;
        int minutes = seconds % 3600 / 60;
        seconds = seconds % 60;
        return $"{(hours < 10 ? '0' : null)}{hours}:{(minutes < 10 ? '0' : null)}{minutes}:{(seconds < 10 ? '0' : null)}{seconds}";
    }
}
[System.Serializable]
public struct Sound
{
    public AudioClip clip;
    [Range(0, 1)] public float volume;
}