using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceManager
{
    static bool debug = false;
    static MyDeviceType debugDeviceType = MyDeviceType.Pc;
    public static bool IsMobile() => debug ? debugDeviceType == MyDeviceType.Mobile : SystemInfo.deviceType == DeviceType.Handheld;
}
[System.Serializable]
public enum MyDeviceType
{
    Mobile,
    Pc
}