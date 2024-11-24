using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Weapon equippedWeapon;  // NPC�� ������ ����
    public float moveSpeed = 3f;  // �̵� �ӵ�
    public float attackRange = 1.5f;  // ���� ����
    public float detectionRange = 5f;  // �÷��̾� Ž�� ����

    private Transform player;
    private float lastAttackTime = 0f;  // ������ ���� �ð�
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            // �÷��̾� ����
            MoveTowardsPlayer();

            if (distanceToPlayer < attackRange && Time.time > lastAttackTime + equippedWeapon.attackSpeed)
            {
                // ���� ������ ������ �� ����
                AttackPlayer();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        // �÷��̾ ���� �̵�
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (animator != null)
        {
            animator.SetBool("isMoving", true);
        }
    }

    void AttackPlayer()
    {
        // ����� ����
        lastAttackTime = Time.time;
        equippedWeapon.Attack(player);  // ����� ����

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }
}
