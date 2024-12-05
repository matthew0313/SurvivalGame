using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HpComp))]
public class HpEffects : MonoBehaviour
{
    HpComp origin;
    private void Awake()
    {
        origin = GetComponent<HpComp>();
        origin.onDamage += OnDamage;
        origin.onHeal += OnHeal;
    }
    void OnDamage(HpChangeData data)
    {
        Color color = Color.white;
        if (data.effectColorType == DamageEffectColorType.Default)
        {
            if (data.amount / origin.maxHp > 0.33f)
            {
                color = new Color(1.0f, 0.0f, 0.0f);
            }
            else if (data.amount / origin.maxHp > 0.2f)
            {
                color = new Color(1.0f, 0.5f, 0.0f);
            }
            else
            {
                color = new Color(1.0f, 1.0f, 0.0f);
            }
        }
        else if (data.effectColorType == DamageEffectColorType.Custom)
        {
            color = data.effectColor;
        }
        string content = data.amount.ToString();
        if (data.amount / origin.maxHp > 0.33f)
        {
            content += "!!";
        }
        else if (data.amount / origin.maxHp > 0.2f)
        {
            content += "!";
        }
        HpEffectsPrefab.Create(transform.position).Set(content, color);
    }
    void OnHeal(HpChangeData data)
    {
        Color color = Color.white;
        if (data.effectColorType == DamageEffectColorType.Default) color = Color.green;
        else if (data.effectColorType == DamageEffectColorType.Custom) color = data.effectColor;
        HpEffectsPrefab.Create(transform.position).Set(data.amount.ToString(), color);
    }
}