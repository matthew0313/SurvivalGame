using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] float minDist = 2.0f, maxDist = 5.0f, reloadingMinDist = 5.0f, reloadingMaxDist = 10.0f;
    [SerializeField] float reloadStartingTime = 2.0f;

    [Header("Speed")]
    [SerializeField] float wanderSpeed;
    [SerializeField] float chaseSpeed, chaseBackstepSpeed;

    [Header("Equipment")]
    [SerializeField] Transform rotator, equipAnchor;
    [SerializeField] EnemyWeapon weapon;

    [Header("Components")]
    [SerializeField] Animator anim;
    Rigidbody2D rb;

    [Header("Drops")]
    [SerializeField] LootTable loot;

    [Header("Field Enemy")]
    [SerializeField] SaveID id;
    public UnityEvent onDeath;

    public Enemy prefabOrigin { get; private set; }
    bool instantiated = false;

    Vector2 originPos;
    Player player;
    HpComp hp;

    bool rotate = false;

    public float detection { get; private set; } = 0;
    readonly int rotXID = Animator.StringToHash("rotX"), rotYID = Animator.StringToHash("rotY");
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
    bool dead = false;
    void OnDeath()
    {
        foreach (var i in loot.GenerateLoot())
        {
            DroppedItem tmp = DroppedItem.Create(transform.position);
            tmp.SetVelocity(Utilities.RandomAngle(0, 360.0f) * 3.0f);
        }
        onDeath?.Invoke();
        if (instantiated)
        {
            Destroy(gameObject);
        }
        else
        {
            dead = true;
            anim.SetBool("Dead", true);
        }
    }
    private void Update()
    {
        if (TimelineCutsceneManager.inCutscene) return;
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
    void Rotate(Vector2 rot)
    {
        float deg = Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg;
        rotator.rotation = Quaternion.Euler(0, 0, deg);
        if (rot.x > 0)
        {
            equipAnchor.localScale = new Vector2(1.0f, 1.0f);
        }
        else if (rot.x < 0)
        {
            equipAnchor.localScale = new Vector2(1.0f, -1.0f);
        }
        anim.SetFloat(rotXID, rot.x); anim.SetFloat(rotYID, rot.y);
    }
    void Move(Vector2 move, float moveSpeed)
    {
        rb.MovePosition(rb.position + move * moveSpeed * Time.deltaTime);
        if (move != Vector2.zero)
        {
            if (!rotate)
            {
                anim.SetFloat(rotXID, move.x); anim.SetFloat(rotYID, move.y);
            }
        }
    }
    public DataUnit Save()
    {
        DataUnit data = new();
        data.floats["posX"] = transform.position.x;
        data.floats["posY"] = transform.position.y;
        data.strings["HpComp"] = JsonUtility.ToJson(hp.Save());
        return data;
    }
    public void Load(DataUnit data)
    {
        transform.position = new Vector2(data.floats["posX"], data.floats["posY"]);
        hp.Load(JsonUtility.FromJson<HpCompSaveData>(data.strings["HpComp"]));
    }
    public void Save(SaveData data)
    {
        if (instantiated) return;
        data.fieldEnemies[id.value] = Save();
    }
    public void Load(SaveData data)
    {
        if (instantiated) return;
        Load(data.fieldEnemies[id.value]);
    }
    protected class EnemyFSMVals : FSMVals
    {

    }
    class TopLayer : TopLayer<Enemy, EnemyFSMVals>
    {
        public TopLayer(Enemy origin, EnemyFSMVals values) : base(origin, values)
        {

        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            origin.weapon.WieldUpdate();
        }
        class Wandering : Layer<Enemy, EnemyFSMVals>
        {
            public Wandering(Enemy origin, Layer<Enemy, EnemyFSMVals> parent) : base(origin, parent)
            {
                defaultState = new Waiting(origin, this);
                AddState("Waiting", defaultState);
                AddState("Moving", new Moving(origin, this));
            }
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                if (origin.weapon.reloading == false) origin.weapon.Reload();
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
            float unscannedCounter = 0.0f;
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if(origin.detection < origin.detectionRequired)
                {
                    parentLayer.ChangeState("Wandering");
                    return;
                }
                if (origin.weapon.reloading)
                {
                    Move(origin.reloadingMinDist, origin.reloadingMaxDist);
                }
                else
                {
                    Move(origin.minDist, origin.maxDist);
                    if (origin.ScanPlayer() && Vector2.Distance(origin.transform.position, origin.player.transform.position) <= origin.weapon.range)
                    {
                        unscannedCounter = 0.0f;
                        origin.weapon.AttemptFire();
                        if (origin.weapon.mag == 0) origin.weapon.Reload();
                    }
                    else
                    {
                        unscannedCounter += Time.deltaTime;
                        if (unscannedCounter > origin.reloadStartingTime) origin.weapon.Reload();
                    }
                }
            }
            void Move(float minDist, float maxDist)
            {
                Vector2 move = (origin.player.transform.position - origin.transform.position).normalized;
                float dist = Vector2.Distance(origin.transform.position, origin.player.transform.position);
                if(dist < minDist)
                {
                    origin.Move(move * -1, origin.chaseBackstepSpeed);
                }
                else if(dist > maxDist)
                {
                    origin.Move(move, origin.chaseSpeed);
                }
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
    public DataUnit data = new();
}
