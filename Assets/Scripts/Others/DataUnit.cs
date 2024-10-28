using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class DataUnit
{
    public SerializableDictionary<string, int> integers;
    public SerializableDictionary<string, float> floats;
    public SerializableDictionary<string, string> strings;
    public DataUnit()
    {
        integers = new();
        floats = new();
        strings = new();
    }
}