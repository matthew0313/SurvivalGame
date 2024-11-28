using UnityEngine;

public class EnemyShotgun : EnemyGun
{
    [SerializeField] int shotCount;
    protected override void Fire()
    {
        for(int i = 0; i < shotCount; i++) base.Fire();
    }
}