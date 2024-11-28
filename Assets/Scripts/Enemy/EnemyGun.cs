using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] HpChangeData damage;
    [SerializeField] protected float fireRate, bulletSpeed, m_range, spread;
    [SerializeField] protected int magSize;
    [SerializeField] protected float reloadTime;
    [SerializeField] Bullet bullet;
    [SerializeField] Transform firePos;
    [SerializeField] Sound fireSound, reloadSound;
    public float range => m_range;
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
    public int mag { get; private set; }
    public void ResetMag()
    {
        mag = magSize;
    }
    readonly int fireID = Animator.StringToHash("Fire");
    public virtual void AttemptFire()
    {
        if (reloading) return;
        if(counter >= fireRate)
        {
            counter = 0.0f;
            mag--;
            AudioManager.Instance.PlaySound(fireSound, transform);
            anim.SetTrigger(fireID);
            Fire();
        }
    }
    protected virtual void Fire()
    {
        bullet.SpawnBullet(firePos.position, firePos.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-spread, spread))).Set(damage, bulletSpeed, range);
    }
    public bool reloading { get; private set; } = false;
    float reloadCounter = 0.0f;
    public bool CanReload() => mag < magSize;
    public void Reload()
    {
        if (reloading) return;
        AudioManager.Instance.PlaySound(reloadSound, transform);
        reloadCounter = 0.0f;
        reloading = true;
    }
    public virtual void WieldUpdate()
    {
        if(counter < fireRate) counter += Time.deltaTime;
        if (reloading)
        {
            reloadCounter += Time.deltaTime;
            if(reloadCounter > reloadTime)
            {
                reloading = false;
                mag = magSize;
            }
        }
    }
}
