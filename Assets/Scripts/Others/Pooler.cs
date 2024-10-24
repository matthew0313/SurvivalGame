using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class Pooler<T> where T : Component
{
    List<T> pool = new();
    readonly int maxSize, defaultSize;
    T prefab;
    public Pooler(T prefab, int maxSize = 100, int defaultSize = 0)
    {
        this.prefab = prefab;
        this.maxSize = maxSize;
        this.defaultSize = defaultSize;
        for (int i = 0; i < defaultSize; i++) 
        {
            T tmp = MonoBehaviour.Instantiate(prefab);
            tmp.gameObject.SetActive(false);
            pool.Add(tmp);
        }
    }
    public T GetObject()
    {
        T obj = Get();
        obj.gameObject.SetActive(true);
        return obj;
    }
    public T GetObject(Vector3 position, Quaternion rotation)
    {
        T obj = Get();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.gameObject.SetActive(true);
        return obj;
    }
    public T GetObject(Vector3 position, Quaternion rotation, Transform parent)
    {
        T obj = Get();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.parent = parent;
        obj.gameObject.SetActive(true);
        return obj;
    }
    public T GetObject(Transform parent)
    {
        T obj = Get();
        obj.transform.parent = parent;
        obj.gameObject.SetActive(true);
        return obj;
    }
    public void ReleaseObject(T obj)
    {
        Release(obj);
    }
    T Get()
    {
        T result;
        pool.RemoveAll((T obj) => obj == null);
        if(pool.Count > 0)
        {
            result = pool[0];
            pool.RemoveAt(0);
        }
        else
        {
            T tmp = MonoBehaviour.Instantiate(prefab);
            tmp.gameObject.SetActive(false);
            result = tmp;
        }
        return result;
    }
    void Release(T obj)
    {
        if(pool.Count >= maxSize)
        {
            MonoBehaviour.Destroy(pool[0].gameObject);
            pool.RemoveAt(0);
        }
        obj.gameObject.SetActive(false);
        pool.Add(obj);
    }
}