using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TabOpener : MonoBehaviour
{
    [Header("TabOpener")]
    [SerializeField] GameObject defaultTab;
    [SerializeField] GameObject m_openTab;
    public GameObject openTab
    {
        get { return m_openTab; }
        set
        {
            if(m_openTab != defaultTab)
            {
                m_openTab.SetActive(false);
            }
            if(value == null || m_openTab == value)
            {
                m_openTab = defaultTab;
            }
            else
            {
                m_openTab = value;
                m_openTab.SetActive(true);
            }
        }
    }
    public bool isTabOpen => m_openTab != null;
}