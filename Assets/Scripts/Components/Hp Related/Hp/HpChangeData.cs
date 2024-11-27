using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public struct HpChangeData
{
    public float amount;
    public DamageEffectColorType effectColorType;
    public Color effectColor;
    public Vector2 knockback;
}
[System.Serializable]
public enum DamageEffectColorType
{
    Default,
    Custom
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HpChangeData))]
public class HpChangeDataDrawer : PropertyDrawer
{
    bool showEffectColor = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, 18.0f), label);
        position.y += 18.0f;
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), property.FindPropertyRelative("amount"));
        position.y += 18.0f;
        SerializedProperty tmp = property.FindPropertyRelative("effectColorType");
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), tmp);
        if (tmp.enumValueIndex == (int)DamageEffectColorType.Custom)
        {
            showEffectColor = true;
            position.y += 18.0f;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), property.FindPropertyRelative("effectColor"));
        }
        else
        {
            showEffectColor = false;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 36.0f + ((showEffectColor) ? 18.0f : 0.0f);
    }
}
#endif