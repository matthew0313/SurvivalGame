using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public abstract class SaveFileHandler
{
    public abstract void Save<T>(T data, string fileName, Action<T> onSave = null) where T : class;
    public abstract void Load<T>(string fileName, Action<T> onLoad = null) where T : class;
    public abstract void Delete(string fileName, Action onDelete = null);
}