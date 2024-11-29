using UnityEngine;

public class EnemyRemington480 : EnemyShotgun
{
    [SerializeField] protected float bleedDamage, bleedDuration;
    protected override Debuff inflictedDebuff => new Bleeding(bleedDuration, bleedDamage);
}