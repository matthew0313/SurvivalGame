using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpDisplayBar : MonoBehaviour, ICutsceneTriggerReceiver
{
    [SerializeField] HpComp origin;
    [SerializeField] Image scaler, backScaler;
    [SerializeField] Color damagedColor = Color.red, healColor = Color.green;
    [SerializeField] float delayTime = 0.5f, scaleSpeed = 5.0f;
    float currentScale, targetScale;
    float barFill => origin.hp / origin.maxHp;
    float counter = 0.0f;
    bool dmgScaling;
    private void Start()
    {
        currentScale = barFill;
        targetScale = currentScale;
        scaler.transform.localScale = new Vector2(currentScale, 1.0f);
        backScaler.transform.localScale = new Vector2(currentScale, 1.0f);
        backScaler.color = damagedColor;
        dmgScaling = true;
        origin.onHpChange += OnHpChange;
    }
    void OnHpChange()
    {
        targetScale = barFill;
        counter = delayTime;
        if (targetScale >= currentScale)
        {
            backScaler.transform.localScale = new Vector2(targetScale, 1.0f);
            scaler.transform.localScale = new Vector2(currentScale, 1.0f);
            if (dmgScaling)
            {
                backScaler.color = healColor;
                dmgScaling = false;
            }
        }
        else if (targetScale < currentScale)
        {
            backScaler.transform.localScale = new Vector2(currentScale, 1.0f);
            scaler.transform.localScale = new Vector2(targetScale, 1.0f);
            if (!dmgScaling)
            {
                backScaler.color = damagedColor;
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
                scaler.transform.localScale = new Vector2(currentScale, 1.0f);
            }
            else if(targetScale < currentScale)
            {
                currentScale = Mathf.Lerp(currentScale, targetScale, scaleSpeed * Time.deltaTime);
                backScaler.transform.localScale = new Vector2(currentScale, 1.0f);
            }
        }
    }

    public void OnCutsceneEnter()
    {
        gameObject.SetActive(false);
    }

    public void OnCutsceneExit()
    {
        gameObject.SetActive(true);
    }
}