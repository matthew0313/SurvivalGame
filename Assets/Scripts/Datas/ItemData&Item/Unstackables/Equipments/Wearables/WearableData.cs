using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;



#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class WearableData : UnstackableItemData
{
    [Header("Wearable")]
    [SerializeField] WearableStats m_stats;
    public WearableStats stats => m_stats;
}
public abstract class Wearable : UnstackableItem
{
    new WearableData data;
    public Wearable(WearableData data) : base(data)
    {
        this.data = data;
    }
    public virtual void OnWear(Player wearer)
    {
        if ((data.stats.traits & WearableTraits.ExtraHp) > 0) wearer.hp.bonusHp += data.stats.extraHealth;
        if ((data.stats.traits & WearableTraits.ExtraSpeed) > 0) wearer.bonusMoveSpeed += data.stats.extraSpeed;
    }
    public virtual void OnUnwear(Player wearer)
    {
        if ((data.stats.traits & WearableTraits.ExtraHp) > 0) wearer.hp.bonusHp -= data.stats.extraHealth;
        if ((data.stats.traits & WearableTraits.ExtraSpeed) > 0) wearer.bonusMoveSpeed -= data.stats.extraSpeed;
    }
}
[System.Serializable]
[Flags]
public enum WearableTraits
{
    ExtraHp = 1<<0,
    ExtraSpeed = 1<<1
}
[System.Serializable]
public struct WearableStats
{
    [SerializeField] WearableTraits m_traits;
    [SerializeField] float m_extraHealth, m_extraSpeed;
    public WearableTraits traits => m_traits;
    public float extraHealth => m_extraHealth;
    public float extraSpeed => m_extraSpeed;
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(WearableStats))]
public class WearableStatsDrawer : PropertyDrawer
{
    int elem = 0;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, 18.0f), label);
        SerializedProperty traitProperty = property.FindPropertyRelative("m_traits");
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), traitProperty);
        elem = 0;
        if ((traitProperty.enumValueFlag & (int)WearableTraits.ExtraHp) > 0)
        {
            position.y += 18;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), property.FindPropertyRelative("m_extraHealth"));
            elem++;
        }
        if ((traitProperty.enumValueFlag & (int)WearableTraits.ExtraSpeed) > 0)
        {
            position.y += 18;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), property.FindPropertyRelative("m_extraSpeed"));
            elem++;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 36.0f + 18.0f * elem;
    }
}
#endif