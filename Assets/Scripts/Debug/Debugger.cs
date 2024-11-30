using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Debugger : MonoBehaviour
{
    [SerializeField] ItemData givingItem;
    [SerializeField] int givingCount;
    [SerializeField] float hpAmount;
    Player player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    public void GiveItemToPlayer()
    {
        if (player == null) return;
        player.AddItem_DropRest(givingItem.Create(), givingCount);
    }
    public void DamagePlayer()
    {
        if (player == null) return;
        player.hp.GetDamage(hpAmount);
    }
    public void HealPlayer()
    {
        if (player == null) return;
        player.hp.Heal(hpAmount);
    }
    public void SetPlayerHp()
    {
        if (player == null) return;
        player.hp.SetHp(hpAmount);
    }
}