using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class HpEffectsPrefab : MonoBehaviour
{
    static Pooler<HpEffectsPrefab> m_pool;
    static Pooler<HpEffectsPrefab> pool { get { if (m_pool == null) m_pool = new Pooler<HpEffectsPrefab>(Resources.Load<HpEffectsPrefab>("HpEffectPrefab")); return m_pool;} }
    [SerializeField] TMP_Text text;
    Color color;
    public static HpEffectsPrefab Create(Vector2 position) => pool.GetObject(position, Quaternion.identity);
    public void Set(string content, Color color)
    {
        this.color = color;
        text.color = color;
        text.text = content;
        StartCoroutine(Disappear(Utilities.RandomAngle(0, 360)));
    }
    const float disappearTime = 0.5f;
    const float speed = 2.0f;
    IEnumerator Disappear(Vector2 dir)
    {
        float counter = disappearTime;
        while(counter > 0.0f)
        {
            transform.Translate(dir * (counter / disappearTime) * speed * Time.deltaTime);
            color.a = counter / disappearTime;
            text.color = color;
            counter -= Time.deltaTime;
            yield return null;
        }
        pool.ReleaseObject(this);
    }
}
