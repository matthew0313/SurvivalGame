using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public UIManager()
    {
        Instance = this;
    }
    [Header("Equipment")]
    [SerializeField] GameObject equipDescContents;
    [SerializeField] Text equippedItemName;
    [SerializeField] Text equippedItemDesc;
    [SerializeField] Transform equippedItemDescBar;

    [Header("Interaction")]
    [SerializeField] Transform pcInteractDisplay;
    [SerializeField] Text pcInteractText;
    [SerializeField] Button mobileInteractButton;
    [SerializeField] Text mobileInteractText;
    [SerializeField] Image mobileInteractImage;

    [Header("Cooldowns")]
    [SerializeField] ConsumableCooldownUI cooldownUIPrefab;
    [SerializeField] Transform cooldownUIAnchor;
    Pooler<ConsumableCooldownUI> cooldownUIpool;

    [Header("Item Grab")]
    [SerializeField] InventorySlotUI grabbingSlotUI;

    [Header("Tabs")]
    [SerializeField] PlayerInventoryUI inventoryUI;
    Player player;
    TopLayer topLayer;
    GameObject m_openTab = null;
    GameObject openTab
    {
        get { return m_openTab; }
        set
        {
            if (value == null)
            {
                if (m_openTab != null) m_openTab.SetActive(false);
                m_openTab = null;
            }
            else
            {
                if (m_openTab != null)
                {
                    m_openTab.SetActive(false);
                }
                if (m_openTab == value) m_openTab = null;
                else
                {
                    m_openTab = value;
                    m_openTab.SetActive(true);
                }
            }
        }
    }
    bool m_canOpenTab = true;
    public bool canOpenTab
    {
        get { return m_canOpenTab; }
        set { m_canOpenTab = value; if (value == false) CloseTab(); }
    }
    public bool isTabOpen => openTab != null;
    public void OpenTab(GameObject tab)
    {
        if (!canOpenTab) return;
        openTab = tab;
    }
    public void CloseTab()
    {
        openTab = null;
    }
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.onEquippedItemChange += EquippedItemChange;
        player.onInteractionChange += SelectedInteractionUpdate;
        topLayer = new TopLayer(this);
        topLayer.OnStateEnter();
        grabbingSlotUI.Set(grabbingSlot);
        cooldownUIpool = new Pooler<ConsumableCooldownUI>(cooldownUIPrefab);
    }
    Item equipDisplaying = null;
    void EquippedItemChange()
    {
        if (equipDisplaying != null) equipDisplaying.onDescUpdate -= EquippedItemDescUpdate;
        equipDisplaying = player.equipped;
        if (equipDisplaying != null)
        {
            equipDescContents.SetActive(true);
            equippedItemName.text = equipDisplaying.data.name;
            equipDisplaying.onDescUpdate += EquippedItemDescUpdate;
            EquippedItemDescUpdate();
        }
        else
        {
            equipDescContents.SetActive(false);
        }
    }
    void EquippedItemDescUpdate()
    {
        equippedItemDesc.text = equipDisplaying.DescBar();
    }
    void SelectedInteractionUpdate()
    {
        if(DeviceManager.IsMobile())
        {
            if (player.currentInteraction != null)
            {
                mobileInteractButton.interactable = true;
                if (player.currentInteraction.interactImage != null)
                {
                    mobileInteractImage.gameObject.SetActive(true);
                    mobileInteractText.gameObject.SetActive(false);
                    mobileInteractImage.sprite = player.currentInteraction.interactImage;
                }
                else
                {
                    mobileInteractImage.gameObject.SetActive(false);
                    mobileInteractText.gameObject.SetActive(true);
                    mobileInteractText.text = player.currentInteraction.interactText;
                }
            }
            else
            {
                mobileInteractButton.interactable = false;
                mobileInteractImage.gameObject.SetActive(false);
                mobileInteractText.gameObject.SetActive(true);
                mobileInteractText.text = "Interact";
            }
        }
        else
        {
            if(player.currentInteraction != null)
            {
                pcInteractDisplay.gameObject.SetActive(true);
                pcInteractDisplay.position = player.currentInteraction.transform.position;
                pcInteractText.text = $"[{InputManager.Instance.interactKey.ToString()}]: {player.currentInteraction.interactText}";
            }
            else
            {
                pcInteractDisplay.gameObject.SetActive(false);
            }
        }
    }
    Dictionary<ConsumableData, ConsumableCooldownUI> cooldownUIs = new();
    List<ConsumableData> displayingCooldowns = new(), displayingCooldownsRemoveQueue = new();
    private void Update()
    {
        if(equipDisplaying != null)
        {
            equippedItemDescBar.localScale = new Vector2(equipDisplaying.DescBarFill(), 1.0f);
        }
        if(!DeviceManager.IsMobile())
        {
            if (Input.GetKeyDown(KeyCode.E)) InventoryTab();
        }
        foreach(var i in displayingCooldowns)
        {
            if (!player.consumableCooldowns.ContainsKey(i))
            {
                cooldownUIs[i].gameObject.SetActive(false);
                displayingCooldownsRemoveQueue.Add(i);
            }
        }
        foreach (var i in displayingCooldownsRemoveQueue) displayingCooldowns.Remove(i);
        displayingCooldowns.Clear();
        foreach(var i in player.consumableCooldowns)
        {
            if (!displayingCooldowns.Contains(i.Key))
            {
                if (!cooldownUIs.ContainsKey(i.Key))
                {
                    ConsumableCooldownUI tmp = Instantiate(cooldownUIPrefab, cooldownUIAnchor);
                    tmp.icon.sprite = i.Key.image;
                    cooldownUIs.Add(i.Key, tmp);
                }
                else cooldownUIs[i.Key].gameObject.SetActive(true);
                displayingCooldowns.Add(i.Key);
            }
            cooldownUIs[i.Key].scaler.localScale = new Vector2(i.Value / i.Key.prefab.cooldown, 1.0f);
            cooldownUIs[i.Key].text.text = Math.Round(i.Value, 1).ToString() + "s";
        }
        topLayer.OnStateUpdate();
    }
    public void InventoryTab()
    {
        OpenTab(inventoryUI.gameObject);
    }
    public List<RaycastResult> scanResults { get; private set; } = new();
    public void ScanUI(Vector2 position)
    {
        PointerEventData dt = new PointerEventData(EventSystem.current);
        dt.position = position;
        EventSystem.current.RaycastAll(dt, scanResults);
    }
    int grabTouchIndex = 0;
    InventorySlot grabbingSlot = new InventorySlot();
    InventorySlot originSlot;
    class TopLayer : TopLayer<UIManager>
    {
        public TopLayer(UIManager origin) : base(origin)
        {
            defaultState = new NoTabOpen(origin, this);
            AddState("NoTabOpen", defaultState);
            AddState("TabOpen", new TabOpen(origin, this));
        }
    }
    class NoTabOpen : State<UIManager>
    {
        public NoTabOpen(UIManager origin, Layer<UIManager> parent) : base(origin, parent)
        {

        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if (origin.isTabOpen) parentLayer.ChangeState("TabOpen");
        }
    }
    class TabOpen : Layer<UIManager>
    {
        public TabOpen(UIManager origin, Layer<UIManager> parent) : base(origin, parent)
        {
            defaultState = new Idle(origin, this);
            AddState("Idle", defaultState);
            AddState("Grabbing", new Grabbing(origin, this));
        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if (!origin.isTabOpen) parentLayer.ChangeState("NoTabOpen");
        }
        class Idle : State<UIManager>
        {
            public Idle(UIManager origin, Layer<UIManager> parent) : base(origin, parent)
            {

            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if(DeviceManager.IsMobile())
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Touch touch = Input.GetTouch(Input.touchCount - 1);
                        if (touch.phase == UnityEngine.TouchPhase.Began)
                        {
                            origin.ScanUI(touch.position);
                            RaycastResult tmp = origin.scanResults.Find((RaycastResult a) => a.gameObject.tag == "InventorySlot");
                            if (tmp.gameObject != null)
                            {
                                InventorySlot touched = tmp.gameObject.GetComponent<InventorySlotUI>().checkingSlot;
                                if (touched.item != null)
                                {
                                    origin.grabTouchIndex = Input.touchCount - 1;
                                    Grab(touched);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        origin.ScanUI(Input.mousePosition);
                        RaycastResult tmp = origin.scanResults.Find((RaycastResult a) => a.gameObject.tag == "InventorySlot");
                        if (tmp.gameObject != null)
                        {
                            InventorySlot clicked = tmp.gameObject.GetComponent<InventorySlotUI>().checkingSlot;
                            if(clicked.item != null) Grab(clicked);
                        }
                    }
                }
            }
            void Grab(InventorySlot grabbed)
            {
                origin.grabbingSlot.SetItem(grabbed.item);
                origin.grabbingSlot.count = grabbed.count;
                grabbed.count = 0;
                origin.originSlot = grabbed;
                parentLayer.ChangeState("Grabbing");
            }
        }
        class Grabbing : State<UIManager>
        {
            public Grabbing(UIManager origin, Layer<UIManager> parent) : base(origin, parent)
            {

            }
            InventorySlotUI highlighting = null;
            InventorySlot grabbingSlot => origin.grabbingSlot;
            InventorySlot originSlot => origin.originSlot;
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                if (DeviceManager.IsMobile())
                {
                    Vector2 pos = Input.GetTouch(origin.grabTouchIndex).position;
                    origin.grabbingSlotUI.transform.position = pos;
                }
                else
                {
                    origin.grabbingSlotUI.transform.position = Input.mousePosition;
                }
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                RaycastResult tmp = new RaycastResult();
                if(DeviceManager.IsMobile())
                {
                    Vector2 pos = Input.GetTouch(origin.grabTouchIndex).position;
                    origin.ScanUI(pos);
                    origin.grabbingSlotUI.transform.position = pos;
                }
                else
                {
                    origin.ScanUI(Input.mousePosition);
                    origin.grabbingSlotUI.transform.position = Input.mousePosition;
                }
                tmp = origin.scanResults.Find((RaycastResult a) => a.gameObject.tag == "InventorySlot");
                if (DeviceManager.IsMobile() ? Input.GetTouch(origin.grabTouchIndex).phase == UnityEngine.TouchPhase.Ended : Input.GetMouseButtonDown(0))
                {
                    if (tmp.gameObject != null)
                    {
                        InventorySlot slot = tmp.gameObject.GetComponent<InventorySlotUI>().checkingSlot;
                        grabbingSlot.count = slot.Insert(grabbingSlot.item, grabbingSlot.count);
                        if (grabbingSlot.count > 0)
                        {
                            if(DeviceManager.IsMobile())
                            {
                                grabbingSlot.count = originSlot.Insert(grabbingSlot.item, grabbingSlot.count);
                                if(grabbingSlot.count > 0)
                                {
                                    grabbingSlot.count = origin.player.inventory.Insert(grabbingSlot.item, grabbingSlot.count);
                                    if (grabbingSlot.count > 0)
                                    {
                                        origin.player.DropItem(Camera.main.ScreenToWorldPoint(origin.grabbingSlotUI.transform.position) - origin.player.transform.position, grabbingSlot.item, grabbingSlot.count);
                                    }
                                    grabbingSlot.count = 0;
                                    parentLayer.ChangeState("Idle");
                                }
                            }
                            else
                            {
                                if(slot.item != null)
                                {
                                    if (!grabbingSlot.item.IsStackable(slot.item) || !slot.item.IsStackable(grabbingSlot.item))
                                    {
                                        grabbingSlot.Swap(slot);
                                    }
                                }
                            }
                        }
                        else
                        {
                            parentLayer.ChangeState("Idle");
                        }
                    }
                    else
                    {
                        origin.player.DropItem(Camera.main.ScreenToWorldPoint(origin.grabbingSlotUI.transform.position) - origin.player.transform.position, grabbingSlot.item, grabbingSlot.count);
                        grabbingSlot.count = 0;
                        parentLayer.ChangeState("Idle");
                    }
                }
                else
                {
                    if (tmp.gameObject != null && (highlighting == null || tmp.gameObject != highlighting.gameObject))
                    {
                        if (highlighting != null) highlighting.UnHighlight();
                        highlighting = tmp.gameObject.GetComponent<InventorySlotUI>();
                        highlighting.Highlight();
                    }
                    else if (tmp.gameObject == null && highlighting != null)
                    {
                        highlighting.UnHighlight();
                        highlighting = null;
                    }
                }
            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                if(highlighting != null)
                {
                    highlighting.UnHighlight();
                    highlighting = null;
                }
                if(grabbingSlot.item != null)
                {
                    origin.player.AddItem_DropRest(grabbingSlot.item, grabbingSlot.count);
                    grabbingSlot.count = 0;
                }
            }
        }
    }
}