using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class DataUnit
{
    [SerializeField] SerializableDictionary<string, int> m_integers;
    [SerializeField] SerializableDictionary<string, float> m_floats;
    [SerializeField] SerializableDictionary<string, bool> m_bools;
    [SerializeField] SerializableDictionary<string, string> m_strings;
    public SerializableDictionary<string, int> integers { get { if (m_integers == null) m_integers = new(); return m_integers; } }
    public SerializableDictionary<string, float> floats { get { if (m_floats == null) m_floats = new(); return m_floats; } }
    public SerializableDictionary<string, bool> bools { get { if (m_bools == null) m_bools = new(); return m_bools; } }
    public SerializableDictionary<string, string> strings { get { if (m_strings == null) m_strings = new(); return m_strings; } }
}