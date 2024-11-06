using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public Action<float> onDamage, onHeal;
    public Action onHpChange, onDeath;
    public List<Func<float, float>> damageModificators { get; } = new();
    public List<Func<float, float>> healModificators { get; } = new();

    private void Awake()
    {
        hp = maxHp;
    }
    bool dead = false;
    
    public void GetDamage(float damage)
    {
        if (dead) return;
        foreach (var i in damageModificators) damage = i.Invoke(damage);
        hp = Mathf.Max(0, hp - damage);
        onDamage?.Invoke(damage);
        onHpChange?.Invoke();
        if(hp <= 0)
        {
            dead = true;
            onDeath?.Invoke();
        }
    }
    public void Heal(float amount)
    {
        if (dead) return;
        foreach (var i in healModificators) amount = i.Invoke(amount);
        hp = Mathf.Min(maxHp, hp + amount);
        onHeal?.Invoke(hp);
        onHpChange?.Invoke();
    }
    public void Revive() => dead = false;
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