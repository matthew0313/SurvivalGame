using UnityEngine;

public class Weapon : MonoBehaviour
{
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
}
