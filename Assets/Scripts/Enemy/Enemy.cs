using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HpComp), typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, ISavable
{
    [Header("AI")]
    [SerializeField] float wanderRange;
    [SerializeField] float detectRange;
    [SerializeField] float detectionRateMin = 1.0f, detectionRateMax = 3.0f;
    [SerializeField] float detectionLowerRate = 2.0f;
    [SerializeField] float maxDetection = 10.0f;
    [SerializeField] float detectionRequired = 1.0f;
    [SerializeField] float minDist = 2.0f, maxDist = 5.0f;

    [Header("Speed")]
    [SerializeField] float wanderSpeed;
    [SerializeField] float chaseSpeed;

    [Header("Equipment")]
    [SerializeField] Transform rotator;
    [SerializeField] EnemyWeapon weapon;

    [Header("Components")]
    [SerializeField] Animator anim;
    Rigidbody2D rb;

    [Header("Drops")]
    [SerializeField] LootTable loot;

    Enemy prefabOrigin;
    bool instantiated = false;

    Vector2 originPos;
    Player player;
    HpComp hp;

    bool rotate = false;

    public float detection { get; private set; } = 0;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        hp = GetComponent<HpComp>();
        hp.onDeath += OnDeath;
        if (!instantiated) originPos = transform.position;
    }
    public Enemy Instantiate()
    {
        Enemy tmp = Instantiate(this);
        tmp.prefabOrigin = this;
        tmp.instantiated = true;
        return tmp;
    }
    public EnemySaveData Save()
    {
        EnemySaveData data = new();
        data.prefab = prefabOrigin;
        data.position = transform.position;
        return data;
    }
    public void Load(EnemySaveData data)
    {
        transform.position = data.position;
    }
    bool ScanPlayer()
    {
        float dist = Vector2.Distance(player.transform.position, transform.position);
        if (dist <= detectRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (player.transform.position - transform.position), Mathf.Infinity, LayerMask.GetMask("Map"));
            if (!hit)
            {
                return true;
            }
        }
        return false;
    }
    void OnDeath()
    {
        foreach (var i in loot.GenerateLoot())
        {
            DroppedItem tmp = DroppedItem.Create(transform.position);
            tmp.SetVelocity(Utilities.RandomAngle(0, 360.0f) * 3.0f);
        }
        if (instantiated)
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        weapon.WieldUpdate();
        ChangeDetection();
    }
    void ChangeDetection()
    {
        if (ScanPlayer())
        {
            detection = Mathf.Min(maxDetection, detection + Mathf.Lerp(detectionRateMin, detectionRateMax, 1.0f - Vector2.Distance(transform.position, player.transform.position) / detectRange) * Time.deltaTime);
        }
        else
        {
            detection = Mathf.Max(0.0f, detection - detectionLowerRate * Time.deltaTime);
        }
    }
    readonly int rotXID = Animator.StringToHash("rotX"), rotYID = Animator.StringToHash("rotY");
    protected class EnemyFSMVals : FSMVals
    {

    }
    class TopLayer : TopLayer<Enemy, EnemyFSMVals>
    {
        public TopLayer(Enemy origin, EnemyFSMVals values) : base(origin, values)
        {

        }
        class Wandering : Layer<Enemy, EnemyFSMVals>
        {
            public Wandering(Enemy origin, Layer<Enemy, EnemyFSMVals> parent) : base(origin, parent)
            {
                defaultState = new Waiting(origin, this);
                AddState("Waiting", defaultState);
                AddState("Moving", new Moving(origin, this));
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (origin.detection >= origin.detectionRequired) parentLayer.ChangeState("Chasing");
            }
            class Waiting : State<Enemy, EnemyFSMVals>
            {
                public Waiting(Enemy origin, Layer<Enemy, EnemyFSMVals> parent) : base(origin, parent)
                {

                }
                float waitTime;
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    waitTime = UnityEngine.Random.Range(2.0f, 10.0f);
                    origin.anim.SetFloat(origin.rotXID, 0.0f); origin.anim.SetFloat(origin.rotYID, 0.0f);
                }
                public override void OnStateFixedUpdate()
                {
                    base.OnStateFixedUpdate();
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0.0f)
                    {
                        parentLayer.ChangeState("Moving");
                    }
                }
            }
            class Moving : State<Enemy, EnemyFSMVals>
            {
                public Moving(Enemy origin, Layer<Enemy, EnemyFSMVals> parent) : base(origin, parent)
                {

                }
                Vector2 targetPos;
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    targetPos = origin.originPos + Utilities.RandomAngle(0, 360.0f) * UnityEngine.Random.Range(0, origin.wanderRange);
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                }
                public override void OnStateFixedUpdate()
                {
                    base.OnStateFixedUpdate();
                    Vector2 rot = (targetPos - (Vector2)origin.transform.position).normalized;
                    origin.anim.SetFloat(origin.rotXID, rot.x); origin.anim.SetFloat(origin.rotYID, rot.y);
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                }
            }
        }
        class Chasing : State<Enemy, EnemyFSMVals>
        {
            public Chasing(Enemy origin, Layer<Enemy, EnemyFSMVals> parent) : base(origin, parent)
            {
            }
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                origin.rotate = true;
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();

            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                origin.rotate = false;
            }
        }
    }
    /*public Weapon equippedWeapon;  // NPC�� ������ ����
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
    }*/
}
[System.Serializable]
public class EnemySaveData
{
    public Enemy prefab;
    public Vector2 position;
    public DataUnit data = new();
}
