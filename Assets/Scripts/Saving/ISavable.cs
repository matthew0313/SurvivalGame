using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public interface ISavable
{
    public void Save(SaveData data);
    public void Load(SaveData data);
}