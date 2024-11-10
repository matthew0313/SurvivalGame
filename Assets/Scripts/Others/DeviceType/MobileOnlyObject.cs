using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileOnlyObject : MonoBehaviour
{
    private void Awake()
    {
        if (!DeviceManager.IsMobile()) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }
}
