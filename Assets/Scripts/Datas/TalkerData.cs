using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Talker Data", menuName = "Scriptables/Cutscene/Talker", order = 0)]
public class TalkerData : ScriptableObject
{
    [SerializeField] Color m_backColor;
    public Color backColor => m_backColor;
}