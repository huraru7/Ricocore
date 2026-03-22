using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Eキーで開閉する「戦車内部情報」メニューのコントローラ。
///
/// 設計:
///   - 各セクション（Inventory / TankStatus / Info / EquipmentSection）を
///     子コンポーネントに委譲し、このクラスは開閉制御のみ担う
///   - 部位スロット UI (4つ) は ModuleSlotUIConfig を使って初期化する
///
/// シーン構成:
///   ModuleMenu (このコンポーネントをアタッチ, 初期非表示)
///   ├── Inventory        → InventoryUI
///   ├── TankStatus       → (次回以降)
///   ├── Info             → ModuleInfoPanel
///   └── EquipmentSection → (次回以降、部位スロット UI を含む)
/// </summary>
public class ModuleMenuUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private TankModuleManager moduleManager;
    [SerializeField] private GameObject        menuRoot;
    [SerializeField] private ModuleInfoPanel   infoPanel;

    [Header("インベントリ")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("部位スロット欄（4つ固定）")]
    [SerializeField] private ModuleSlotUI slotTurret;
    [SerializeField] private ModuleSlotUI slotEngine;
    [SerializeField] private ModuleSlotUI slotRightCaterpillar;
    [SerializeField] private ModuleSlotUI slotLeftCaterpillar;

    private bool isOpen;

    // 部位ごとの色
    private static readonly Color ColorTurret           = new Color(0.29f, 0.56f, 0.85f); // 青
    private static readonly Color ColorEngine           = new Color(0.96f, 0.65f, 0.14f); // 黄
    private static readonly Color ColorRightCaterpillar = new Color(0.82f, 0.01f, 0.11f); // 赤
    private static readonly Color ColorLeftCaterpillar  = new Color(0.49f, 0.82f, 0.13f); // 緑

    // -------------------------------------------------------

    void Awake()
    {
        menuRoot.SetActive(false);

        // 部位スロット UI を Config 形式で初期化（クリック → 取り外してインベントリへ）
        slotTurret.Initialize(new ModuleSlotUIConfig
        {
            Slot              = moduleManager.PartSlots[TankSlot.Turret],
            InfoPanel         = infoPanel,
            OnClick           = _ => moduleManager.UnequipFromPart(TankSlot.Turret),
            Label             = "砲塔",
            ActionButtonLabel = "取り外し",
            SlotColor         = ColorTurret
        });

        slotEngine.Initialize(new ModuleSlotUIConfig
        {
            Slot              = moduleManager.PartSlots[TankSlot.Engine],
            InfoPanel         = infoPanel,
            OnClick           = _ => moduleManager.UnequipFromPart(TankSlot.Engine),
            Label             = "エンジン",
            ActionButtonLabel = "取り外し",
            SlotColor         = ColorEngine
        });

        slotRightCaterpillar.Initialize(new ModuleSlotUIConfig
        {
            Slot              = moduleManager.PartSlots[TankSlot.RightCaterpillar],
            InfoPanel         = infoPanel,
            OnClick           = _ => moduleManager.UnequipFromPart(TankSlot.RightCaterpillar),
            Label             = "右キャタピラー",
            ActionButtonLabel = "取り外し",
            SlotColor         = ColorRightCaterpillar
        });

        slotLeftCaterpillar.Initialize(new ModuleSlotUIConfig
        {
            Slot              = moduleManager.PartSlots[TankSlot.LeftCaterpillar],
            InfoPanel         = infoPanel,
            OnClick           = _ => moduleManager.UnequipFromPart(TankSlot.LeftCaterpillar),
            Label             = "左キャタピラー",
            ActionButtonLabel = "取り外し",
            SlotColor         = ColorLeftCaterpillar
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
            inventoryUI.ResetFilter(); // メニューを開いたら ALL タブに戻す
        else
            infoPanel.Hide();
    }
}
