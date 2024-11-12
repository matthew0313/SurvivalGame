using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class EquipmentData : UnstackableItemData
{
    [Header("Equipment")]
    [SerializeField] MaxDurabilityData m_durability;
    public MaxDurabilityData durability => m_durability;
}
public abstract class Equipment : Item
{
    new EquipmentData data;
    public float maxDurability => data.durability.value;
    public float durability;
    public Equipment(EquipmentData data) : base(data)
    {
        this.data = data;
        durability = data.durability.value;
    }
    public Action onDurabilityChange;
    public void DurabilityReduce(float amount)
    {
        if (data.durability.mode == DurabilityMode.Unbreakable) return;
        durability = Mathf.Max(0, durability - amount);
        onDurabilityChange?.Invoke();
        if(durability <= 0)
        {
            OnBreak();
            containedSlot.count = 0;
        }
    }
    protected virtual void OnBreak()
    {

    }
    public override Item Copy()
    {
        Equipment item = base.Copy() as Equipment;
        item.durability = durability;
        return item;
    }
    public override void Save(DataUnit data)
    {
        base.Save(data);
        data.floats.Add("Durability", durability);
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        if (data.floats.TryGetValue("Durability", out float tmp))
        {
            durability = tmp;
            onDurabilityChange?.Invoke();
        }
    }
}
[System.Serializable]
public struct MaxDurabilityData
{
    [SerializeField] DurabilityMode m_mode;
    [SerializeField] int m_value;
    public DurabilityMode mode => m_mode;
    public int value => mode == DurabilityMode.Default ? m_value : 100;
}
[System.Serializable]
public enum DurabilityMode
{
    Default,
    Unbreakable
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MaxDurabilityData))]
public class MaxDurabilityDataDrawer : PropertyDrawer
{
    bool showDurability = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty modeProperty = property.FindPropertyRelative("m_mode");
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), modeProperty, label);
        if(modeProperty.enumValueIndex == (int)DurabilityMode.Default)
        {
            position.y += 18.0f;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), property.FindPropertyRelative("m_value"));
            showDurability = true;
        }
        else
        {
            showDurability = false;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return showDurability ? 36.0f : 18.0f;
    }
}
#endif