using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpComp : MonoBehaviour
{
    [SerializeField] float m_maxHp;
    public float maxHp => m_maxHp;
    public float hp { get; private set; }
    private void Awake()
    {
        
    }
}
