using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public abstract class SaveFileHandler
{
    public abstract void Save<T>(T data, string fileName) where T : class;
    public abstract T Load<T>(string fileName) where T : class;
    public abstract void Delete(string fileName);
}