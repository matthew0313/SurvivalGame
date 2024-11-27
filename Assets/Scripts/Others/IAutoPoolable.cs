using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;


public interface IAutoPoolable<T> where T : Component
{
    T prefabOrigin { get; set; }
    Pooler<T> pool { get; set; }
    bool instantiated { get; set; }
    public T Instantiate()
    {
        if (instantiated) return null;
        if (pool == null) pool = new Pooler<T>(this as T, 100, 0, (T tmp) => { }, (T tmp) => { });
        T tmp = pool.GetObject();
        (tmp as IAutoPoolable<T>).instantiated = true;
        (tmp as IAutoPoolable<T>).prefabOrigin = this as T;
        return tmp;
    }
    public virtual void OnInstantiate(T obj)
    {
        (obj as T).gameObject.SetActive(true);
    }
    public void Release()
    {
        if (!instantiated) return;
        pool.ReleaseObject(this as T);
    }
    public virtual void OnRelease(T obj)
    {
        (obj as T).gameObject.SetActive(false);
    }
}