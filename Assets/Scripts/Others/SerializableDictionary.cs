using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    public List<TKey> keys = new();
    public List<TValue> values = new();
    public void OnAfterDeserialize()
    {
        if(keys.Count != values.Count)
        {
            Debug.LogError("Key Value count mismatch");
        }
        else
        {
            for(int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }
        }
    }

    public void OnBeforeSerialize()
    {
        foreach(var i in this)
        {
            keys.Add(i.Key);
            values.Add(i.Value);
        }
    }
}