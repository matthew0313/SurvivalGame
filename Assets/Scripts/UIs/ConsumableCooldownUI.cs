using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableCooldownUI : MonoBehaviour
{
    [SerializeField] Image m_icon;
    [SerializeField] Transform m_scaler;
    [SerializeField] Text m_text;
    public Image icon => m_icon;
    public Transform scaler => m_scaler;
    public Text text => m_text;
}