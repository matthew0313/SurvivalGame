using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] HpComp origin;
    [SerializeField] Transform scaler, damageScaler, healScaler;
    [SerializeField] float delayTime = 0.5f, scaleSpeed = 5.0f;
    float currentScale, targetScale;
    float barFill => origin.hp / origin.maxHp;
    float counter = 0.0f;
    bool dmgScaling;
    private void Start()
    {
        currentScale = barFill;
        targetScale = currentScale;
        scaler.localScale = new Vector2(currentScale, scaler.localScale.y);
        damageScaler.localScale = new Vector2(currentScale, damageScaler.localScale.y);
        damageScaler.gameObject.SetActive(true);
        healScaler.gameObject.SetActive(false);
        dmgScaling = true;
        origin.onHpChange += OnHpChange;
    }
    void OnHpChange()
    {
        targetScale = barFill;
        counter = delayTime;
        if (targetScale > currentScale)
        {
            healScaler.localScale = new Vector2(targetScale, healScaler.localScale.y);
            scaler.localScale = new Vector2(currentScale, scaler.localScale.y);
            if (dmgScaling)
            {
                healScaler.gameObject.SetActive(true);
                damageScaler.gameObject.SetActive(false);
                dmgScaling = false;
            }
        }
        else if (targetScale < currentScale)
        {
            damageScaler.localScale = new Vector2(currentScale, damageScaler.localScale.y);
            scaler.localScale = new Vector2(targetScale, scaler.localScale.y);
            if (!dmgScaling)
            {
                healScaler.gameObject.SetActive(false);
                damageScaler.gameObject.SetActive(true);
                dmgScaling = true;
            }
        }
    }
    private void Update()
    {
        if (counter > 0.0f) counter -= Time.deltaTime;
        if (counter <= 0.0f)
        {
            if(targetScale > currentScale)
            {
                currentScale = Mathf.Lerp(currentScale, targetScale, scaleSpeed * Time.deltaTime);
                scaler.localScale = new Vector2(currentScale, scaler.localScale.y);
            }
            else if(targetScale < currentScale)
            {
                currentScale = Mathf.Lerp(currentScale, targetScale, scaleSpeed * Time.deltaTime);
                damageScaler.localScale = new Vector2(currentScale, damageScaler.localScale.y);
            }
        }
    }
}