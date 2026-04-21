using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// Eキーで開閉する「戦車内部情報」メニューのコントローラ（UI Toolkit 版）。
///
/// UXML: Assets/UI/UXML/ModuleMenu.uxml
/// USS:  Assets/UI/USS/Common.uss  (.module-menu / .part-slots)
///
/// 設計:
///   - 部位スロット 4 つを ModuleSlot.uxml から CloneTree() で生成
///   - Inventory は別 UIDocument（InventoryUI）を SetVisible() で連動制御
///   - 開閉は panel.style.display で行う（SetActive 不要）
///
/// uGUI 版との主な違い:
///   [SerializeField] GameObject menuRoot        → Q("menu-root").style.display
///   [SerializeField] ModuleSlotUI slotTurret 等 → CloneTree() で動的生成
///   menuRoot.SetActive()                        → panel.style.display + inventoryUI.SetVisible()
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class ModuleMenuUI : MonoBehaviour
{
    [Header("連動パネル（別 UIDocument）")]
    [SerializeField] private InventoryUI    inventoryUI;
    [SerializeField] private TankStatsPanel tankStatsPanel;

    [Header("スロットテンプレート（ModuleSlot.uxml を設定）")]
    [SerializeField] private VisualTreeAsset slotTemplate;

    // ---- VisualElement 参照 ----
    private VisualElement panel;

    // 部位スロットコントローラ
    private ModuleSlotUI slotTurret;
    private ModuleSlotUI slotEngine;
    private ModuleSlotUI slotRightCaterpillar;
    private ModuleSlotUI slotLeftCaterpillar;

    private bool isOpen;

    // -------------------------------------------------------

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("menu-root");
        panel.style.display = DisplayStyle.None;
    }

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        slotTurret           = CreatePartSlot(root.Q("slot-turret-wrap"), SlotType.Turret,           "砲塔");
        slotEngine           = CreatePartSlot(root.Q("slot-engine-wrap"), SlotType.Engine,           "エンジン");
        slotRightCaterpillar = CreatePartSlot(root.Q("slot-right-wrap"),  SlotType.RightCaterpillar, "右キャタピラー");
        slotLeftCaterpillar  = CreatePartSlot(root.Q("slot-left-wrap"),   SlotType.LeftCaterpillar,  "左キャタピラー");
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            ToggleMenu();
    }

    // -------------------------------------------------------

    private ModuleSlotUI CreatePartSlot(VisualElement container, SlotType part, string label)
    {
        if (container == null) return null;

        var instance = slotTemplate.CloneTree();
        container.Add(instance);

        var slotUI = new ModuleSlotUI(instance);
        slotUI.Initialize(new ModuleSlotUIConfig
        {
            Slot              = PlayerSystemHub.Instance.EquipSystem.GetPartSlot(part),
            InfoPanel         = ModuleInfoPanel.Instance,
            OnClick           = _ => PlayerSystemHub.Instance.EquipSystem.TryUnequip(part),
            Label             = label,
            ActionButtonLabel = "取り外し",
            SlotColor         = SlotTypeColor.Get(part),
        });

        return slotUI;
    }

    private void ToggleMenu()
    {
        isOpen = !isOpen;
        panel.style.display = isOpen ? DisplayStyle.Flex : DisplayStyle.None;
        inventoryUI?.SetVisible(isOpen);
        tankStatsPanel?.SetVisible(isOpen);

        if (isOpen)
            inventoryUI?.ResetFilter();
        else
            ModuleInfoPanel.Instance?.Hide();
    }
}
