using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Eキーで開閉する「戦車内部情報」メニューのコントローラ。
///
/// 設計:
///   - 部位スロット UI (4つ) と インベントリスロット UI (inventorySize 個) を Awake で初期化
///   - 各 ModuleSlotUI は ModuleSlot.OnChanged を購読しており、変化時に自動で表示を更新する
///   - このクラスは手動更新を一切行わず、コネクション設定のみ担う
///
/// シーン構成:
///   ModuleMenuRoot (このコンポーネントをアタッチ, 初期非表示)
///   ├── Panel_Parts
///   │   ├── SlotUI_Turret        → ModuleSlotUI
///   │   ├── SlotUI_Engine        → ModuleSlotUI
///   │   ├── SlotUI_Right         → ModuleSlotUI
///   │   └── SlotUI_Left          → ModuleSlotUI
///   ├── Panel_Inventory
///   │   └── SlotUI_Inv_0 〜 N   → ModuleSlotUI (inventorySlotUIs 配列に設定)
///   └── InfoPanel               → ModuleInfoPanel
/// </summary>
public class ModuleMenuUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private TankModuleManager moduleManager;
    [SerializeField] private GameObject        menuRoot;
    [SerializeField] private ModuleInfoPanel   infoPanel;

    [Header("部位スロット欄（4つ固定）")]
    [SerializeField] private ModuleSlotUI slotTurret;
    [SerializeField] private ModuleSlotUI slotEngine;
    [SerializeField] private ModuleSlotUI slotRightCaterpillar;
    [SerializeField] private ModuleSlotUI slotLeftCaterpillar;

    [Header("インベントリスロット欄（TankModuleManager の inventorySize と同数を設定）")]
    [SerializeField] private ModuleSlotUI[] inventorySlotUIs;

    private bool isOpen;

    // -------------------------------------------------------

    void Awake()
    {
        menuRoot.SetActive(false);

        // 部位スロット UI を初期化
        // クリック → 取り外してインベントリへ
        slotTurret.Initialize(
            moduleManager.PartSlots[TankSlot.Turret],   infoPanel,
            _ => moduleManager.UnequipFromPart(TankSlot.Turret),
            "砲塔");

        slotEngine.Initialize(
            moduleManager.PartSlots[TankSlot.Engine],   infoPanel,
            _ => moduleManager.UnequipFromPart(TankSlot.Engine),
            "エンジン");

        slotRightCaterpillar.Initialize(
            moduleManager.PartSlots[TankSlot.RightCaterpillar], infoPanel,
            _ => moduleManager.UnequipFromPart(TankSlot.RightCaterpillar),
            "右キャタピラー");

        slotLeftCaterpillar.Initialize(
            moduleManager.PartSlots[TankSlot.LeftCaterpillar],  infoPanel,
            _ => moduleManager.UnequipFromPart(TankSlot.LeftCaterpillar),
            "左キャタピラー");

        // インベントリスロット UI を初期化
        // UI 数とスロット数が合わない場合は小さい方に合わせる
        int count = System.Math.Min(inventorySlotUIs.Length, moduleManager.InventorySlots.Length);
        if (inventorySlotUIs.Length != moduleManager.InventorySlots.Length)
        {
            Debug.LogWarning($"[ModuleMenuUI] インベントリスロット UI 数 ({inventorySlotUIs.Length}) と " +
                             $"TankModuleManager のスロット数 ({moduleManager.InventorySlots.Length}) が異なります。");
        }

        for (int i = 0; i < count; i++)
        {
            var invSlot = moduleManager.InventorySlots[i];
            var uiSlot  = inventorySlotUIs[i];
            // クリック → 互換部位スロットへ自動装着
            uiSlot.Initialize(invSlot, infoPanel,
                slot => moduleManager.AutoEquip(slot),
                (i + 1).ToString());
        }
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

        // メニューを閉じた時に情報パネルも隠す
        if (!isOpen)
            infoPanel.Hide();
    }
}
