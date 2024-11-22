using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float followRange = 10f; // 따라가는 범위
    public float attackRange = 1.5f; // 공격 범위
    public float speed = 2f; // 이동 속도
    public float attackCooldown = 1f; // 공격 쿨다운 시간

    private Transform player;
    private float lastAttackTime = 0;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= followRange && distanceToPlayer > attackRange)
        {
            // 플레이어를 따라감
            FollowPlayer();
        }
        else if (distanceToPlayer <= attackRange)
        {
            // 플레이어 공격
            AttackPlayer();
        }
    }

    void FollowPlayer()
    {
        // 플레이어 방향으로 이동
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        // 공격 로직 (쿨다운 포함)
        if (Time.time - lastAttackTime > attackCooldown)
        {
            Debug.Log("적이 공격합니다!");
            lastAttackTime = Time.time;
            // 공격 구현 (예: 플레이어 체력 감소)
        }
    }

    void OnDrawGizmosSelected()
    {
        // 에디터에서 추적 및 공격 범위를 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}