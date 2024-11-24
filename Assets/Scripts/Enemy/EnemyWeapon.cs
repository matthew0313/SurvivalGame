using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;  // 무기 이름
    public int attackPower;  // 공격력
    public float attackSpeed;  // 공격 속도

    // 무기 공격 메서드
    public void Attack(Transform target)
    {
        // 여기서는 예시로 공격력을 출력합니다. 실제로는 플레이어의 체력 등을 감소시키는 로직이 필요합니다.
        Debug.Log(weaponName + "로 공격! 공격력: " + attackPower);
        // 예시: 플레이어의 체력 감소
        if (target.GetComponent<PlayerHealth>() != null)
        {
            target.GetComponent<PlayerHealth>().TakeDamage(attackPower);
        }
    }
}
