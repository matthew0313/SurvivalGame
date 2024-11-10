using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;

public static class ExtensionMethods
{
    public static int EnumCount<EnumT>(this int target) where EnumT : Enum
    {
        if(target == -1) return Enum.GetNames(typeof(EnumT)).Length;
        else
        {
            int count = 0;
            while(target != 0)
            {
                if ((target & 1) == 1) count++;
                target = target >> 1;
            }
            return count;
        }
    }
}