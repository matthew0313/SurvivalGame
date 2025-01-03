using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(Inventory), typeof(HpComp))]
public class Player : MonoBehaviour, ISavable, ICutsceneTriggerReceiver
{
    [SerializeField] Transform model;

    [Header("Input")]
    [SerializeField] InputActionReference moveInput;
    [SerializeField] InputActionReference aimInput;
    [SerializeField] Button interactInput;
    [SerializeField] UtilityButton useButton;

    [Header("Movement")]
    [SerializeField] float m_moveSpeed;
    public float bonusMoveSpeed = 0.0f;
    public float moveSpeed { get { return Mathf.Max(0.1f, m_moveSpeed + bonusMoveSpeed); } }
    public bool canMove = true;
    public bool rotate = false;

    [Header("Components")]
    [SerializeField] Animator anim;
    Rigidbody2D rb;

    [Header("Inventory")]
    [SerializeField] ItemDataIntPair[] startContents = new ItemDataIntPair[16];
    [SerializeField] Transform m_equipAnchor, rotator;
    public Transform equipAnchor => m_equipAnchor;
    Inventory m_inventory = null;
    HpComp m_hp;
    public Inventory inventory { get { if (m_inventory == null) m_inventory = GetComponent<Inventory>(); return m_inventory; } }
    public HpComp hp { get { if (m_hp == null) m_hp = GetComponent<HpComp>(); return m_hp; } }

    public readonly InventorySlot clothingSlot = new InventorySlot((Item item) => item is Clothing);
    public readonly InventorySlot accessorySlot1 = new InventorySlot((Item item) => item is Accessory), accessorySlot2 = new InventorySlot((Item item) => item is Accessory);
    public readonly InventorySlot backpackSlot = new InventorySlot((Item item) => item is Backpack);

    public readonly Dictionary<ConsumableData, float> consumableCooldowns = new();

    public readonly List<Interaction> interactions = new();
    public bool canInteract = true;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        for (int i = 0; i < inventory.slotCount; i++)
        {
            if (startContents[i].item == null) continue;
            inventory.slots[i].Insert(startContents[i].item.Create(), startContents[i].count);
        }
        clothingSlot.onItemChange += ClothingChange;
        accessorySlot1.onItemChange += Accessory1Change;
        accessorySlot2.onItemChange += Accessory2Change;
        backpackSlot.onItemChange += BackpackChange;
        hp.onDeath += OnDeath;
    }
    public bool dead { get; private set; } = false;
    public Action onDeath;
    void OnDeath()
    {
        dead = true;
        anim.SetBool("Dead", true);
        onDeath?.Invoke();
        AudioManager.Instance.FadeMusic(2.0f, null);
        StartCoroutine(DeathAnim());
    }
    IEnumerator DeathAnim()
    {
        float counter = 0.0f;
        if (GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>().profile.TryGet(out Bloom bloom))
        {
            float startVal = bloom.intensity.value;
            while (counter < 3.0f)
            {
                counter += Time.deltaTime;
                bloom.intensity.value = startVal + 800.0f * counter / 3.0f;
                yield return null;
            }
        }
        GameManager.Instance.GameOver();
    }
    public void OnCutsceneEnter()
    {
        currentInteraction = null;
        onInteractionChange?.Invoke();
        equipAnchor.gameObject.SetActive(false);
    }
    public void OnCutsceneExit()
    {
        equipAnchor.gameObject.SetActive(true);
    }
    private void Start()
    {
        equippedSlot.onItemChange += OnEquippedSlotUpdate;
        OnEquippedSlotUpdate();
    }
    private void Update()
    {
        if (TimelineCutsceneManager.inCutscene || dead || GameManager.Instance.paused) return;
        RotateCheck();
        EquipUpdate();
        CooldownUpdate();
        InteractionUpdate();
    }
    private void FixedUpdate()
    {
        if (TimelineCutsceneManager.inCutscene || dead || GameManager.Instance.paused) return;
        Move(moveInput.action.ReadValue<Vector2>());
    }
    Vector2 lastMovedDirection = Vector2.right;
    readonly int rotXID = Animator.StringToHash("rotX"), rotYID = Animator.StringToHash("rotY");
    readonly int movingID = Animator.StringToHash("Moving");
    void Move(Vector2 move)
    {
        if (!canMove) return;
        rb.MovePosition(rb.position + move * moveSpeed * Time.deltaTime);
        if (move != Vector2.zero)
        {
            lastMovedDirection = move.normalized;
            anim.SetBool(movingID, true);
            if (!rotate)
            {
                anim.SetFloat(rotXID, move.x); anim.SetFloat(rotYID, move.y);
            }
        }
        else
        {
            anim.SetBool(movingID, false);
        }
    }
    void RotateCheck()
    {
        if (!rotate)
        {
            rotator.rotation = Quaternion.identity;
        }
        else
        {
            if (DeviceManager.IsMobile())
            {
                if (InputManager.UseButton()) Rotate(InputManager.AimInput(rotator.position));
            }
            else Rotate(InputManager.AimInput(rotator.position));
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
    public Action onEquippedItemChange;
    public int equippedIndex { get; private set; } = 0;
    InventorySlot equippedSlot => inventory.slots[equippedIndex];
    public Item equipped { get; private set; } = null;
    public void SwapSlot(int index)
    {
        if (index > 6 || index < 0 || index == equippedIndex) return;
        equippedSlot.onItemChange -= OnEquippedSlotUpdate;
        equippedIndex = index;
        equippedSlot.onItemChange += OnEquippedSlotUpdate;
        OnEquippedSlotUpdate();
    }
    Clothing clothing = null;
    Accessory accessory1 = null, accessory2 = null;
    Backpack backpack = null;
    void ClothingChange()
    {
        if (clothing != null) clothing.OnUnwear(this);
        clothing = clothingSlot.item as Clothing;
        if (clothing != null) clothing.OnWear(this);
    }
    void Accessory1Change()
    {
        if (accessory1 != null) accessory1.OnUnwear(this);
        accessory1 = accessorySlot1.item as Accessory;
        if (accessory1 != null) accessory1.OnWear(this);
    }
    void Accessory2Change()
    {
        if (accessory2 != null) accessory2.OnUnwear(this);
        accessory2 = accessorySlot2.item as Accessory;
        if (accessory2 != null) accessory2.OnWear(this);
    }
    void BackpackChange()
    {
        if (backpack != null) backpack.OnUnwear(this);
        backpack = backpackSlot.item as Backpack;
        if (backpack != null) backpack.OnWear(this);
    }
    void OnEquippedSlotUpdate()
    {
        if (equipped != null) equipped.OnUnwield();
        equipped = equippedSlot.item;
        if (equipped != null) equipped.OnWield(this, equippedSlot);
        onEquippedItemChange?.Invoke();
    }
    void EquipUpdate()
    {
        if(!DeviceManager.IsMobile())
        {
            for(int i = 0; i < 6; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SwapSlot(i);
                    break;
                }
            }
        }
        if (equipped != null)
        {
            equipped.OnWieldUpdate();
        }
    }
    List<ConsumableData> updateQueue = new();
    void CooldownUpdate()
    {
        updateQueue = consumableCooldowns.Keys.ToList();
        foreach(var i in updateQueue)
        {
            consumableCooldowns[i] -= Time.deltaTime;
            if (consumableCooldowns[i] <= 0) consumableCooldowns.Remove(i);
        }
    }
    public void AddItem_DropRest(Vector2 dir, Item item, int count)
    {
        int remaining = inventory.Insert(item, count);
        if (remaining > 0) DropItem(dir, item, remaining);
    }
    public void AddItem_DropRest(Item item, int count)
    {
        int remaining = inventory.Insert(item, count);
        if (remaining > 0) DropItem(Utilities.RandomAngle(0, 360), item, remaining);
    }
    const float itemDropForce = 5.0f;
    public void DropItem(Vector2 dir, Item item, int count)
    {
        DroppedItem tmp = DroppedItem.Create(transform.position);
        tmp.Set(item, count);
        tmp.SetVelocity(dir.normalized * itemDropForce);
    }
    public void Interact()
    {
        if (!canInteract || currentInteraction == null) return;
        currentInteraction.OnInteract();
        if (currentInteraction != null && currentInteraction.removeUponInteract) RemoveInteraction(currentInteraction);
    }
    [SerializeField] Interaction m_currentInteraction = null;
    public Interaction currentInteraction
    {
        get { return m_currentInteraction; }
        private set
        {
            m_currentInteraction = value;
            onInteractionChange?.Invoke();
        }
    }
    public Action onInteractionChange;
    void InteractionUpdate()
    {
        interactions.Sort((Interaction a, Interaction b) =>
        {
            if (a.canInteract)
            {
                if (b.canInteract)
                {
                    return Vector2.Distance(a.transform.position, transform.position).CompareTo(Vector2.Distance(b.transform.position, transform.position));
                }
                else return -1;
            }
            else
            {
                if (b.canInteract) return 1;
                else return 0;
            }
        });
        if(interactions.Count > 0 && interactions[0].canInteract)
        {
            if (interactions[0] != currentInteraction)
            {
                if (currentInteraction != null) currentInteraction.OnDeselect();
                currentInteraction = interactions[0];
                currentInteraction.OnSelect();
            }
        }
        else
        {
            if(currentInteraction != null)
            {
                currentInteraction.OnDeselect();
                currentInteraction = null;
            }
        }
        if (InputManager.InteractButtonDown()) Interact(); 
    }
    public void AddInteraction(Interaction interaction)
    {
        if (interactions.Contains(interaction)) return;
        interactions.Add(interaction);
    }
    public void RemoveInteraction(Interaction interaction)
    {
        if (!interactions.Contains(interaction)) return;
        interactions.Remove(interaction);
    }
    public Action<Vector2> onTeleport;
    public void Teleport(Vector2 position)
    {
        transform.position = position;
        onTeleport?.Invoke(position);
    }
    public void Save(SaveData data)
    {
        data.player.equipmentData[0] = clothingSlot.Save();
        data.player.equipmentData[1] = accessorySlot1.Save();
        data.player.equipmentData[2] = accessorySlot2.Save();
        data.player.equipmentData[3] = backpackSlot.Save();
        data.player.inventoryData = inventory.Save();
        data.player.hpData = hp.Save();
        data.player.position = transform.position;
        foreach (var i in consumableCooldowns) data.player.consumableCooldowns.Add(i.Key, i.Value);
    }

    public void Load(SaveData data)
    {
        clothingSlot.Load(data.player.equipmentData[0]);
        accessorySlot1.Load(data.player.equipmentData[1]);
        accessorySlot2.Load(data.player.equipmentData[2]);
        backpackSlot.Load(data.player.equipmentData[3]);
        inventory.Load(data.player.inventoryData);
        hp.Load(data.player.hpData);
        transform.position = data.player.position;
        foreach (var i in data.player.consumableCooldowns) consumableCooldowns.Add(i.Key, i.Value);
    }
}
[System.Serializable]
public class PlayerSaveData
{
    public InventorySaveData inventoryData = new();
    public HpCompSaveData hpData = new();
    public InventorySlotSaveData[] equipmentData = new InventorySlotSaveData[4];
    public SerializableDictionary<ConsumableData, float> consumableCooldowns = new();
    public Vector2 position;
}