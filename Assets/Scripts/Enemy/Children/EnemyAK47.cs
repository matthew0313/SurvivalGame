using UnityEngine;

public class EnemyAK47 : EnemyGun
{
    [SerializeField] protected float bleedDamage, bleedDuration;
    protected override Debuff inflictedDebuff => new Bleeding(bleedDuration, bleedDamage);
}