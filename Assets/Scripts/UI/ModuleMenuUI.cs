using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Eキーで開閉する「戦車内部情報」メニューのコントローラ。
///
/// 設計:
///   - 各セクション（Inventory / TankStatus / Info / EquipmentSection）を
///     子コンポーネントに委譲し、このクラスは開閉制御のみ担う
///   - 部位スロット UI は EquipSystem.GetPartSlot() で取得した PartSlot を使う
///   - 部位スロット UI は ModuleSlotUIConfig を使って初期化する
///
/// Canvas にアタッチすること（常にアクティブな親に置く必要がある）
/// </summary>
public class ModuleMenuUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private EquipSystem     equipSystem;
    [SerializeField] private GameObject      menuRoot;
    [SerializeField] private ModuleInfoPanel infoPanel;

    [Header("インベントリ")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("部位スロット欄（4つ固定、未設定は自動スキップ）")]
    [SerializeField] private ModuleSlotUI slotTurret;
    [SerializeField] private ModuleSlotUI slotEngine;
    [SerializeField] private ModuleSlotUI slotRightCaterpillar;
    [SerializeField] private ModuleSlotUI slotLeftCaterpillar;

    private bool isOpen;

    // -------------------------------------------------------

    void Awake()
    {
        menuRoot.SetActive(false);

        // 部位スロット UI を Config 形式で初期化（null のものは自動スキップ）
        InitializePartSlot(slotTurret,           SlotType.Turret,           "砲塔");
        InitializePartSlot(slotEngine,           SlotType.Engine,           "エンジン");
        InitializePartSlot(slotRightCaterpillar, SlotType.RightCaterpillar, "右キャタピラー");
        InitializePartSlot(slotLeftCaterpillar,  SlotType.LeftCaterpillar,  "左キャタピラー");
    }

    private void InitializePartSlot(ModuleSlotUI ui, SlotType part, string label)
    {
        if (ui == null) return; // 未設定のスロットはスキップ

        ui.Initialize(new ModuleSlotUIConfig
        {
            Slot              = equipSystem.GetPartSlot(part),
            InfoPanel         = infoPanel,
            OnClick           = _ => equipSystem.TryUnequip(part),
            Label             = label,
            ActionButtonLabel = "取り外し",
            SlotColor         = SlotTypeColor.Get(part)
        });
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            ToggleMenu();
    }

    private void ToggleMenu()
    {
        isOpen = !isOpen;
        menuRoot.SetActive(isOpen);

        if (isOpen)
            inventoryUI.ResetFilter();    // メニューを開いたら ALL タブに戻す
        else if (infoPanel != null)
            infoPanel.Hide();
    }
}
