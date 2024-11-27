using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class Debuff
{
    float duration;
    protected HpComp debuffed;
    public abstract DebuffType type { get; }
    public abstract Sprite debuffIcon { get; }
    protected abstract DebuffEffectPrefab effectPrefab { get; }
    public Debuff() { }
    public Debuff(float duration)
    {
        this.duration = duration;
    }
    public static Debuff GetDebuff(DebuffType type)
    {
        if (type == DebuffType.Bleeding) return new Bleeding();
        else return null;
    }
    public void InflictTo(HpComp debuffed)
    {
        if ((debuffed.immunities & type) != 0) return;
        Debuff found = debuffed.debuffs.Find((Debuff tmp) => tmp.type == type);
        if (found != null) found.ReApply(this);
        else
        {
            this.debuffed = debuffed;
            debuffed.AddDebuff(this);
        }
    }
    public virtual void ReApply(Debuff reapplied)
    {
        duration = Mathf.Max(duration, reapplied.duration);
    }
    static readonly Dictionary<DebuffEffectPrefab, Pooler<DebuffEffectPrefab>> pools = new();
    DebuffEffectPrefab effect = null;
    public virtual void OnDebuffAdd()
    {
        if (!pools.ContainsKey(effectPrefab)) pools.Add(effectPrefab, new Pooler<DebuffEffectPrefab>(effectPrefab));
        effect = pools[effectPrefab].GetObject(debuffed.transform.position, Quaternion.identity, debuffed.transform);
    }
    public virtual void OnDebuffUpdate()
    {
        duration = Mathf.Max(duration - Time.deltaTime, 0.0f);
        if(duration <= 0.0)
        {
            RemoveDebuff();
        }
    }
    bool removed = false;
    public void RemoveDebuff()
    {
        if (removed) return;
        removed = true;
        debuffed.RemoveDebuff(this);
    }
    public virtual void OnDebuffRemove()
    {
        pools[effectPrefab].ReleaseObject(effect);
    }
    public virtual DebuffSaveData Save()
    {
        DebuffSaveData data = new();
        data.type = type;
        data.data.floats["duration"] = duration;
        return data;
    }
    public virtual void Load(DataUnit data)
    {
        duration = data.floats["duration"];
    }
}
[System.Serializable]
[Flags]
public enum DebuffType
{
    Bleeding = 1<<0
}
[System.Serializable]
public class DebuffSaveData
{
    public DebuffType type;
    public DataUnit data = new();
}