using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class HpComp : MonoBehaviour
{
    [Header("HpComp")]
    [SerializeField] float baseHp;
    float m_bonusHp = 0;
    public float hp { get; private set; }
    public float bonusHp
    {
        get { return m_bonusHp; }
        set
        {
            m_bonusHp = value;
            if (hp > maxHp) hp = maxHp;
            onHpChange?.Invoke();
        }
    }
    public float maxHp => baseHp + bonusHp;

    public Action<HpChangeData> onDamage, onHeal;
    public Action onHpChange, onDeath;
    public List<Func<HpChangeData, HpChangeData>> damageModificators { get; } = new();
    public List<Func<HpChangeData, HpChangeData>> healModificators { get; } = new();

    private void Awake()
    {
        hp = maxHp;
    }
    bool dead = false;
    
    public void GetDamage(float damage)
    {
        GetDamage(new HpChangeData() { amount = damage });
    }
    public void GetDamage(HpChangeData data)
    {
        if (dead) return;
        foreach (var i in damageModificators) data = i.Invoke(data);
        hp = Mathf.Max(0, hp - data.amount);
        onDamage?.Invoke(data);
        onHpChange?.Invoke();
        if (hp <= 0)
        {
            dead = true;
            onDeath?.Invoke();
        }
    }
    public void Heal(float amount)
    {
        Heal(new HpChangeData() { amount = amount });
    }
    public void Heal(HpChangeData data)
    {
        if (dead) return;
        foreach (var i in healModificators) data = i.Invoke(data);
        hp = Mathf.Min(maxHp, hp + data.amount);
        onHeal?.Invoke(data);
        onHpChange?.Invoke();
    }
    public void Revive() => dead = false;
    public HpCompSaveData Save()
    {
        HpCompSaveData data = new();
        data.hp = hp;
        return data;
    }
    public void Load(HpCompSaveData data)
    {
        hp = data.hp;
        onHpChange?.Invoke();
    }
}
[System.Serializable]
public class HpCompSaveData
{
    public float hp;
}
#if UNITY_EDITOR
[CustomEditor(typeof(HpComp))]
public class HpComp_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Give Damage")) (target as HpComp).GetDamage(10.0f);
        if (GUILayout.Button("Heal")) (target as HpComp).Heal(10.0f);
    }
}
#endif