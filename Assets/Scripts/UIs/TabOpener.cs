using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.InputSystem;

public class TabOpener : MonoBehaviour
{
    [SerializeField] GameObject defaultTab;
    GameObject openTab;
    private void OnEnable()
    {
        if(openTab != null) openTab.SetActive(false);
        openTab = defaultTab;
        openTab.SetActive(true);
    }
    public void OpenTab(GameObject tab)
    {
        if (openTab == tab) return;
        openTab.SetActive(false);
        openTab = tab;
        openTab.SetActive(true);
    }
}