using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Action<float> onDamage;
    public Action onHpChange, onDeath;

    private void Awake()
    {
        hp = maxHp;
    }
    bool dead = false;
    
    public void GetDamage(float damage)
    {
        if (dead) return;
        hp = Mathf.Max(0, hp - damage);
        onDamage?.Invoke(damage);
        onHpChange?.Invoke();
        if(hp <= 0)
        {
            dead = true;
            onDeath?.Invoke();
        }
    }
}
