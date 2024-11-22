using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float followRange = 10f; // ���󰡴� ����
    public float attackRange = 1.5f; // ���� ����
    public float speed = 2f; // �̵� �ӵ�
    public float attackCooldown = 1f; // ���� ��ٿ� �ð�

    private Transform player;
    private float lastAttackTime = 0;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= followRange && distanceToPlayer > attackRange)
        {
            // �÷��̾ ����
            FollowPlayer();
        }
        else if (distanceToPlayer <= attackRange)
        {
            // �÷��̾� ����
            AttackPlayer();
        }
    }

    void FollowPlayer()
    {
        // �÷��̾� �������� �̵�
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        // ���� ���� (��ٿ� ����)
        if (Time.time - lastAttackTime > attackCooldown)
        {
            Debug.Log("���� �����մϴ�!");
            lastAttackTime = Time.time;
            // ���� ���� (��: �÷��̾� ü�� ����)
        }
    }

    void OnDrawGizmosSelected()
    {
        // �����Ϳ��� ���� �� ���� ������ �ð�ȭ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}