using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceManager : MonoBehaviour
{
    public static DeviceManager Instance { get; private set; }
    public DeviceManager()
    {
        Instance = this;
    }
    [SerializeField] bool debug = false;
    [SerializeField] MyDeviceType debugDeviceType = MyDeviceType.Pc;
    public static bool IsMobile() => Instance.debug ? Instance.debugDeviceType == MyDeviceType.Mobile : SystemInfo.deviceType == DeviceType.Handheld;
}
[System.Serializable]
public enum MyDeviceType
{
    Mobile,
    Pc
}