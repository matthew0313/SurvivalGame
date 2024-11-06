using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpText : MonoBehaviour
{
    [SerializeField] HpComp origin;
    [SerializeField] Text text;
    private void Awake()
    {
        origin.onHpChange += () => text.text = $"{origin.hp}/{origin.maxHp}";
    }
}