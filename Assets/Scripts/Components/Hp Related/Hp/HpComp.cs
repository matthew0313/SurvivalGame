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
    [SerializeField] Alliance m_alliance;
    [SerializeField] float baseHp;
    [SerializeField] DebuffType m_immunities;

    [Header("Knockback")]
    [SerializeField] bool knockbackImmune = false;
    public Action<Vector2> onKnockbackReceive;

    float m_bonusHp = 0;
    public Alliance alliance => m_alliance;
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
    public DebuffType immunities => m_immunities;

    public Action<HpChangeData> onDamage, onHeal;
    public Action onHpChange, onDeath;
    public List<Func<HpChangeData, HpChangeData>> damageModificators { get; } = new();
    public List<Func<HpChangeData, HpChangeData>> healModificators { get; } = new();

    private void Awake()
    {
        hp = maxHp;
    }
    public bool dead { get; private set; } = false;
    
    public void GetDamage(float damage)
    {
        GetDamage(new HpChangeData() { amount = damage });
    }
    public void GetDamage(HpChangeData data)
    {
        if (dead) return;
        foreach (var i in damageModificators) data = i.Invoke(data);
        hp = Mathf.Max(0, hp - data.amount);
        if (data.knockback.magnitude > 0.0f) onKnockbackReceive?.Invoke(data.knockback);
        onDamage?.Invoke(data);
        onHpChange?.Invoke();
        if (hp <= 0)
        {
            dead = true;
            onDeath?.Invoke();
        }
    }
    public readonly List<Debuff> debuffs = new();
    readonly List<Debuff> removeQueue = new();
    public void AddDebuff(Debuff debuff)
    {
        debuffs.Add(debuff);
        debuff.OnDebuffAdd();
    }
    public void RemoveDebuff(Debuff debuff)
    {
        removeQueue.Add(debuff);
    }
    private void Update()
    {
        foreach (var i in debuffs) i.OnDebuffUpdate();
        if(removeQueue.Count > 0)
        {
            foreach (var i in removeQueue)
            {
                debuffs.Remove(i);
                i.OnDebuffRemove();
            }
            removeQueue.Clear();
        }
    }
    public void Heal(float amount)
    {
        Heal(new HpChangeData() { amount = amount });
    }
    public void FullHeal() => Heal(maxHp);
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
        foreach(var i in debuffs)
        {
            data.debuffs.Add(i.Save());
        }
        return data;
    }
    public void Load(HpCompSaveData data)
    {
        hp = data.hp;
        foreach(var i in data.debuffs)
        {
            Debuff tmp = Debuff.GetDebuff(i.type);
            if(tmp != null)
            {
                tmp.Load(i.data);
                tmp.InflictTo(this);
            }
        }
        onHpChange?.Invoke();
        if (hp == 0) dead = true;
    }
}
[System.Serializable]
public class HpCompSaveData
{
    public float hp;
    public List<DebuffSaveData> debuffs = new();
}
[System.Serializable]
[Flags]
public enum Alliance
{
    Player = 1<<0,
    Enemy = 1<<1
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
        if (GUILayout.Button("Inflict burn")) new Bleeding(30.0f, 3.0f).InflictTo(target as HpComp);
    }
}
#endif