using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
public class Debugger : MonoBehaviour
{
    [SerializeField] ItemData givingItem;
    [SerializeField] int givingCount;
    [SerializeField] float hpAmount;
    [SerializeField] Sound changingMusic;
    [SerializeField] TimelineAsset cutscene;
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
    MusicPriorityPair debugMusic;
    public void ForceChangeMusic()
    {
        if (AudioManager.Instance == null) return;
        if (debugMusic == null)
        {
            debugMusic = new MusicPriorityPair() { music = changingMusic, priority = 9999 };
        }
        else debugMusic.music = changingMusic;
        AudioManager.Instance.AddMusic(debugMusic);
    }
    public void PlayCutscene()
    {
        if(TimelineCutsceneManager.Instance == null) return;
        TimelineCutsceneManager.Instance.PlayCutscene(cutscene);
    }
}