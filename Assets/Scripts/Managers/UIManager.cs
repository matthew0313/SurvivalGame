using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public UIManager()
    {
        Instance = this;
    }
    [SerializeField] Animator anim;
    readonly int InCutsceneID = Animator.StringToHash("InCutscene");
    readonly int TalkingID = Animator.StringToHash("Talking");
    readonly int TalkOpenID = Animator.StringToHash("TalkOpen");

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

    [Header("Cutscenes")]
    [SerializeField] GameObject defaultUI;
    [SerializeField] GameObject cutsceneUI;
    [SerializeField] List<Cutscene> cutscenes = new();
    [SerializeField] Image talkTalkerImage, talkTalkerBackImage;
    [SerializeField] Color talkTalkerBackDefaultColor;
    [SerializeField] Text talkTalker, talkContent;
    [SerializeField] float talkRate, talkPauseTime;

    [Header("Pause")]
    [SerializeField] RectTransform pauseMenu;
    [SerializeField] Image pauseImage;
    [SerializeField] Sprite pausedSprite, notPausedSprite;

    [Header("Others")]
    [SerializeField] RectTransform blackBack;
    Cutscene currentCutscene;

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
        TimelineCutsceneManager.onCutsceneEnter += OnCutsceneEnter;
        TimelineCutsceneManager.onCutsceneExit += OnCutsceneExit;
        GameManager.Instance.onPauseToggle += OnPauseToggle;
        cooldownUIpool = new Pooler<ConsumableCooldownUI>(cooldownUIPrefab);
    }
    Item equipDisplaying = null;
    void OnCutsceneEnter()
    {
        defaultUI.gameObject.SetActive(false);
        cutsceneUI.gameObject.SetActive(true);
    }
    void OnCutsceneExit()
    {
        defaultUI.gameObject.SetActive(true);
        cutsceneUI.gameObject.SetActive(false);
    }
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
    public void OnPauseToggle()
    {
        if (GameManager.Instance.paused)
        {
            pauseImage.sprite = pausedSprite;
            GameManager.Instance.canTogglePause = false;
            pauseMenu.gameObject.SetActive(true);
            pauseMenu.DOAnchorPosY(0.0f, 0.5f).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() =>
            {
                GameManager.Instance.canTogglePause = true;
            });
        }
        else
        {
            pauseImage.sprite = notPausedSprite;
            GameManager.Instance.canTogglePause = false;
            pauseMenu.DOAnchorPosY(1100.0f, 0.5f).SetEase(Ease.InCirc).SetUpdate(true).OnComplete(() =>
            {
                GameManager.Instance.canTogglePause = true;
                pauseMenu.gameObject.SetActive(false);
            });
        }
    }
    public void InventoryTab()
    {
        OpenTab(inventoryUI.gameObject);
    }
    public void StartCutscene(int cutsceneIndex) => currentCutscene = cutscenes[cutsceneIndex];
    public List<RaycastResult> scanResults { get; private set; } = new();
    public void ScanUI(Vector2 position)
    {
        PointerEventData dt = new PointerEventData(EventSystem.current);
        dt.position = position;
        EventSystem.current.RaycastAll(dt, scanResults);
    }
    bool blackingOut = false;
    public void BlackOut(Action onFullBlack)
    {
        if (blackingOut) return;
        blackingOut = true;
        blackBack.pivot = new Vector2(1.0f, 0.5f);
        blackBack.DOScaleX(1.0f, 0.5f).SetEase(Ease.InCirc).OnComplete(() =>
        {
            onFullBlack?.Invoke();
            blackBack.pivot = new Vector2(0.0f, 0.5f);
            blackBack.DOScaleX(0.0f, 0.5f).SetEase(Ease.InCirc).OnComplete(() =>
            {
                blackingOut = false;
            });
        });
    }
    int grabTouchIndex = 0;
    InventorySlot grabbingSlot = new InventorySlot();
    InventorySlot originSlot;
    IEnumerator movingCamera = null;
    void MoveCamera(Vector2 targetPos, Action onMoveFinish = null)
    {
        if (movingCamera != null) StopCoroutine(movingCamera);
        movingCamera = MovingCamera(targetPos, onMoveFinish);
        StartCoroutine(movingCamera);
    }
    const float cameraMoveSpeed = 10.0f;
    IEnumerator MovingCamera(Vector2 targetPos, Action onMoveFinish)
    {
        while(Vector2.Distance((Vector2)Camera.main.transform.position, targetPos) > 0.1f)
        {
            Camera.main.transform.Translate(Vector2.ClampMagnitude((targetPos - (Vector2)Camera.main.transform.position) * cameraMoveSpeed * Time.deltaTime, Vector2.Distance((Vector2)Camera.main.transform.position, targetPos)));
            yield return null;
        }
        onMoveFinish?.Invoke();
    }
    class UIVals : FSMVals
    {
        public int cutsceneElementIndex;
    }
    class TopLayer : TopLayer<UIManager, UIVals>
    {
        public TopLayer(UIManager origin) : base(origin, new UIVals())
        {
            defaultState = new Default(origin, this);
            AddState("Default", defaultState);
            AddState("InCutscene", new InCutscene(origin, this));
        }
    }
    class Default : Layer<UIManager, UIVals>
    {
        public Default(UIManager origin, Layer<UIManager, UIVals> parent) : base(origin, parent)
        {
            defaultState = new NoTabOpen(origin, this);
            AddState("NoTabOpen", defaultState);
            AddState("TabOpen", new TabOpen(origin, this));
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            origin.defaultUI.SetActive(true);
        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if(origin.currentCutscene != null)
            {
                parentLayer.ChangeState("InCutscene");
            }
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            origin.defaultUI.SetActive(false);
        }
        class NoTabOpen : State<UIManager, UIVals>
        {
            public NoTabOpen(UIManager origin, Layer<UIManager, UIVals> parent) : base(origin, parent)
            {

            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (origin.isTabOpen) parentLayer.ChangeState("TabOpen");
            }
        }
        class TabOpen : Layer<UIManager, UIVals>
        {
            public TabOpen(UIManager origin, Layer<UIManager, UIVals> parent) : base(origin, parent)
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
            class Idle : State<UIManager, UIVals>
            {
                public Idle(UIManager origin, Layer<UIManager, UIVals> parent) : base(origin, parent)
                {

                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if (DeviceManager.IsMobile())
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
                                if (clicked.item != null) Grab(clicked);
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
            class Grabbing : State<UIManager, UIVals>
            {
                public Grabbing(UIManager origin, Layer<UIManager, UIVals> parent) : base(origin, parent)
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
                    if (DeviceManager.IsMobile())
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
                                if (DeviceManager.IsMobile())
                                {
                                    grabbingSlot.count = originSlot.Insert(grabbingSlot.item, grabbingSlot.count);
                                    if (grabbingSlot.count > 0)
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
                                    if (slot.item != null)
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
                    if (highlighting != null)
                    {
                        highlighting.UnHighlight();
                        highlighting = null;
                    }
                    if (grabbingSlot.item != null)
                    {
                        origin.player.AddItem_DropRest(grabbingSlot.item, grabbingSlot.count);
                        grabbingSlot.count = 0;
                    }
                }
            }
        }
    }
    class InCutscene : Layer<UIManager, UIVals>
    {
        public InCutscene(UIManager origin, Layer<UIManager, UIVals> parent) : base(origin, parent)
        {
            AddState("Talk", new Talk(origin, this));
        }
        public override void OnStateEnter()
        {
            origin.anim.SetBool(origin.InCutsceneID, true);
            Camera.main.transform.SetParent(null);
            values.cutsceneElementIndex = 0;
            if (origin.currentCutscene.elements[0].type == CutsceneElementType.Talk)
            {
                currentState = states["Talk"];
            }
            currentState.OnStateEnter();
        }
        public override void RefreshState()
        {
            base.RefreshState();
            values.cutsceneElementIndex++;
            if (values.cutsceneElementIndex >= origin.currentCutscene.elements.Count)
            {
                parentLayer.ChangeState("Default");
            }
            else
            {
                if (origin.currentCutscene.elements[values.cutsceneElementIndex].type == CutsceneElementType.Talk)
                {
                    ChangeState("Talk");
                }
                else if(origin.currentCutscene.elements[values.cutsceneElementIndex].type == CutsceneElementType.CameraMove)
                {
                    ChangeState("CameraMove");
                }
            }
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            origin.anim.SetBool(origin.InCutsceneID, false);
            Camera.main.transform.SetParent(origin.player.transform);
            Camera.main.transform.localPosition = new Vector3(0, 0, -10);
            origin.currentCutscene = null;
        }
        class Talk : State<UIManager, UIVals>
        {
            public Talk(UIManager origin, Layer<UIManager, UIVals> parent) : base(origin, parent)
            {

            }
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                origin.StartCoroutine(Talking(origin.currentCutscene.elements[values.cutsceneElementIndex].talk));
            }
            int talkProgressIndex = 0;
            IEnumerator talkProgress = null;
            IEnumerator Talking(CutsceneTalk content)
            {
                origin.talkTalkerBackImage.color = origin.talkTalkerBackDefaultColor;
                origin.talkTalkerImage.gameObject.SetActive(false);
                origin.anim.SetBool(origin.TalkingID, true);
                while (origin.anim.GetBool(origin.TalkOpenID) == false) yield return null;
                foreach (var i in content.elements)
                {
                    origin.talkContent.text = "";
                    if (i.talkerImage != null)
                    {
                        origin.talkTalkerImage.gameObject.SetActive(true);
                        origin.talkTalkerImage.sprite = i.talkerImage;
                    }
                    else
                    {
                        origin.talkTalkerImage.gameObject.SetActive(false);
                    }
                    origin.talkTalker.text = i.talker;
                    if (i.talkerBackColorType == CutsceneTalkTalkerBackColorType.Default)
                    {
                        origin.talkTalkerBackImage.color = origin.talkTalkerBackDefaultColor;
                    }
                    else
                    {
                        origin.talkTalkerBackImage.color = new Color(i.talkerBackColor.r, i.talkerBackColor.g, i.talkerBackColor.b, origin.talkTalkerBackImage.color.a);
                    }
                    talkProgress = TalkProgress(i);
                    origin.StartCoroutine(talkProgress);
                    while (talkProgress != null)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            origin.StopCoroutine(talkProgress);
                            origin.talkContent.text = i.dialogue;
                            yield return null;
                            break;
                        }
                        yield return null;
                    }
                    while (!Input.GetMouseButtonDown(0)) yield return null;
                }
                origin.anim.SetBool(origin.TalkingID, false);
                parentLayer.RefreshState();
            }
            IEnumerator TalkProgress(CutsceneTalkElement content)
            {
                for (int i = 0; i < content.dialogue.Length; i++)
                {
                    origin.talkContent.text += content.dialogue[i];
                    if (content.dialogue[i] == '.' || content.dialogue[i] == ',' || content.dialogue[i] == '?' || content.dialogue[i] == '!') yield return new WaitForSeconds(origin.talkPauseTime);
                    else yield return new WaitForSeconds(origin.talkRate);
                }
                talkProgress = null;
            }
        }
        class CameraMove : State<UIManager, UIVals>
        {
            public CameraMove(UIManager origin, Layer<UIManager, UIVals> parent) : base(origin, parent)
            {

            }
            public override void OnStateEnter()
            {
                base.OnStateEnter();
            }
        }
    }
}
[System.Serializable]
public class Cutscene
{
    [SerializeField] List<CutsceneElement> m_elements;
    public List<CutsceneElement> elements => m_elements;
}
[System.Serializable]
public enum CutsceneElementType
{
    Talk,
    CameraMove
}
[System.Serializable]
public struct CutsceneElement
{
    [SerializeField] CutsceneElementType m_type;
    [SerializeField] CutsceneTalk m_talk;
    [SerializeField] CameraMove m_cameraMove;
    public CutsceneElementType type => m_type;
    public CutsceneTalk talk => m_talk;
    public CameraMove cameraMove => m_cameraMove;
}
[System.Serializable]
public struct CutsceneTalk
{
    [SerializeField] List<CutsceneTalkElement> m_elements;
    public List<CutsceneTalkElement> elements => m_elements;
}
[System.Serializable]
public struct CameraMove
{
    [SerializeField] Vector2 m_position;
    public Vector2 position => m_position;
}
[System.Serializable]
public enum CutsceneTalkTalkerBackColorType
{
    Default,
    Custom
}
[System.Serializable]
public struct CutsceneTalkElement
{
    [SerializeField] string m_talker;
    [SerializeField] Sprite m_talkerImage;
    [SerializeField] CutsceneTalkTalkerBackColorType m_talkerBackColorType;
    [SerializeField] Color m_talkerBackColor;
    [TextArea][SerializeField] string m_dialogue;
    [SerializeField] CameraMove[] cameraMoves;
    public string talker => m_talker;
    public Sprite talkerImage => m_talkerImage;
    public CutsceneTalkTalkerBackColorType talkerBackColorType => m_talkerBackColorType;
    public Color talkerBackColor => m_talkerBackColor;
    public string dialogue => m_dialogue;
}
#if UNITY_EDITOR
/*[CustomPropertyDrawer(typeof(CutsceneTalkElement))]
public class CutsceneTalkElementDrawer : PropertyDrawer
{
    bool displayColor = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), property.FindPropertyRelative("m_talker"));
        position.y += 19.0f;
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), property.FindPropertyRelative("m_talkerImage"));
        position.y += 19.0f;
        SerializedProperty talkerBackColorTypeProperty = property.FindPropertyRelative("m_talkerBackColorType");
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), talkerBackColorTypeProperty);
        if (talkerBackColorTypeProperty.enumValueIndex == (int)CutsceneTalkTalkerBackColorType.Custom)
        {
            displayColor = true;
            position.y += 19.0f;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 18.0f), property.FindPropertyRelative("m_talkerBackColor"));
        }
        else displayColor = false;
        position.y += 19.0f;
        SerializedProperty dialogueProperty = property.FindPropertyRelative("m_dialogue");
        EditorGUI.TextArea(new Rect(position.x, position.y, position.width, 36.0f), dialogueProperty.stringValue);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 95.0f + (displayColor ? 19.0f : 0.0f);
    }
}*/
[CustomEditor(typeof(UIManager))]
public class UIManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Cutscene 0")) (target as UIManager).StartCutscene(0);
    }
}
#endif