using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] HpChangeData damage;
    [SerializeField] protected float fireRate, bulletSpeed, bulletRange, spread;
    [SerializeField] protected int magSize;
    [SerializeField] Bullet bullet;
    [SerializeField] Transform firePos;
    /*
    public string weaponName;  // ���� �̸�
    public int attackPower;  // ���ݷ�
    public float attackSpeed;  // ���� �ӵ�

    // ���� ���� �޼���
    public void Attack(Transform target)
    {
        // ���⼭�� ���÷� ���ݷ��� ����մϴ�. �����δ� �÷��̾��� ü�� ���� ���ҽ�Ű�� ������ �ʿ��մϴ�.
        Debug.Log(weaponName + "�� ����! ���ݷ�: " + attackPower);
        // ����: �÷��̾��� ü�� ����
        if (target.GetComponent<PlayerHealth>() != null)
        {
            target.GetComponent<PlayerHealth>().TakeDamage(attackPower);
        }
    }
    */
    float counter = 0.0f;
    int mag;
    private void Awake()
    {
        mag = magSize;
    }
    public virtual void AttemptFire()
    {
        if(counter >= fireRate)
        {
            counter = 0.0f;
            Fire();
        }
    }
    protected virtual void Fire()
    {
        bullet.SpawnBullet(firePos.position, firePos.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-spread, spread))).Set(damage, bulletSpeed, bulletRange);
    }
    public virtual void WieldUpdate()
    {
        if(counter < fireRate) counter += Time.deltaTime;
    }
}
