using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public abstract class SaveFileHandler
{
    public abstract void Save(string data, string fileName, Action onSave = null);
    public abstract void Load(string fileName, Action<string> onLoad = null);
    public abstract void Delete(string fileName, Action onDelete = null);
}