using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : Interaction, ISavable
{
    [SerializeField] LootTable loot;
    [SerializeField] TimeVal cooldown;
    [SerializeField] Animator chestAnim;
    [SerializeField] SaveID id;
    bool isOnCooldown = false;
    float cooldownLeft = 0.0f;
    public override bool canInteract => !isOnCooldown;
    public override string interactText => "Open Chest";
    readonly int openID = Animator.StringToHash("Open");

    private void OnValidate()
    {
        if (!gameObject.scene.IsValid()) id.value = null;
        else if (string.IsNullOrEmpty(id.value)) id.SetNew();
    }
    public override void OnInteract()
    {
        base.OnInteract();
        if (isOnCooldown) return;
        foreach(var i in loot.GenerateLoot())
        {
            DroppedItem item = DroppedItem.Create(transform.position);
            item.Set(i.item, i.count);
            item.SetVelocity(Utilities.RandomAngle(180, 360) * 5.0f);
        }
        isOnCooldown = true;
        chestAnim.SetBool(openID, true);
        cooldownLeft = cooldown.time;
    }
    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownLeft = Mathf.Max(0, cooldownLeft - Time.deltaTime);
            if(cooldownLeft <= 0)
            {
                isOnCooldown = false;
                chestAnim.SetBool(openID, false);
            }
        }
    }

    public void Save(SaveData data)
    {
        DataUnit tmp = new();
        tmp.bools["isOnCooldown"] = isOnCooldown;
        tmp.floats["cooldownLeft"] = cooldownLeft;
        data.interactions[id.value] = tmp;
    }

    public void Load(SaveData data)
    {
        if (data.interactions.TryGetValue(id.value, out DataUnit tmp))
        {
            if(tmp.bools.TryGetValue("isOnCooldown", out bool tmp2))
            {
                isOnCooldown = tmp2;
                tmp.floats.TryGetValue("cooldownLeft", out cooldownLeft);
                chestAnim.SetBool(openID, isOnCooldown);
            }
        }
    }
}