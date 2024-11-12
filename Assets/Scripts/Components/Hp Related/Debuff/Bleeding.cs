using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bleeding : Debuff
{
    float damagePerSecond;
    public override DebuffType type => DebuffType.Bleeding;
    public override Sprite debuffIcon => Resources.Load<Sprite>("Debuffs/Icons/Bleeding");
    protected override DebuffEffectPrefab effectPrefab => Resources.Load<DebuffEffectPrefab>("Debuffs/Effects/Bleeding");
    public Bleeding() : base() { }
    public Bleeding(float duration, float damagePerSecond) : base(duration)
    {
        this.damagePerSecond = damagePerSecond;
    }
    float counter = 0.0f;
    public override void ReApply(Debuff reapplied)
    {
        
    }
    public override void OnDebuffUpdate()
    {
        counter += Time.deltaTime;
        if(counter >= 1.0f)
        {
            counter = 0.0f;
            debuffed.GetDamage(new HpChangeData() { amount = damagePerSecond, effectColorType = DamageEffectColorType.Custom, effectColor = new Color(0.9f, 0.0f, 0.0f) });
        }
        base.OnDebuffUpdate();
    }
    public override DebuffSaveData Save()
    {
        DebuffSaveData data = base.Save();
        data.data.floats["damagePerSecond"] = damagePerSecond;
        return data;
    }
    public override void Load(DataUnit data)
    {
        base.Load(data);
        damagePerSecond = data.floats["damagePerSecond"];

    }
}