using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Weapon equippedWeapon;  // NPC가 장착한 무기
    public float moveSpeed = 3f;  // 이동 속도
    public float attackRange = 1.5f;  // 공격 범위
    public float detectionRange = 5f;  // 플레이어 탐지 범위

    private Transform player;
    private float lastAttackTime = 0f;  // 마지막 공격 시간
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            // 플레이어 추적
            MoveTowardsPlayer();

            if (distanceToPlayer < attackRange && Time.time > lastAttackTime + equippedWeapon.attackSpeed)
            {
                // 공격 범위에 들어왔을 때 공격
                AttackPlayer();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        // 플레이어를 향해 이동
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (animator != null)
        {
            animator.SetBool("isMoving", true);
        }
    }

    void AttackPlayer()
    {
        // 무기로 공격
        lastAttackTime = Time.time;
        equippedWeapon.Attack(player);  // 무기로 공격

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }
}
