using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Chest : Interaction, ISavable
{
    [SerializeField] LootTable loot;
    [SerializeField] TimeVal cooldown;
    [SerializeField] Animator chestAnim;
    [SerializeField] SaveID id;
    public UnityEvent onChestOpen;
    bool isOnCooldown = false;
    float cooldownLeft = 0.0f;
    public override bool canInteract => !isOnCooldown;
    public override string interactText => "Open Chest";
    readonly int openID = Animator.StringToHash("Open");
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
        tmp.floats["cooldownLeft"] = cooldownLeft;
        data.mapObjects[id.value] = tmp;
    }

    public void Load(SaveData data)
    {
        DataUnit tmp = data.mapObjects[id.value];
        cooldownLeft = tmp.floats["cooldownLeft"];
        if (cooldownLeft > 0)
        {
            isOnCooldown = true;
            chestAnim.SetBool(openID, true);
        }
    }
}