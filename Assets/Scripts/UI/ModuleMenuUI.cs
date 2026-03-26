using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Eキーで開閉する「戦車内部情報」メニューのコントローラ。
///
/// 設計:
///   - 各セクション（Inventory / TankStatus / Info / EquipmentSection）を
///     子コンポーネントに委譲し、このクラスは開閉制御のみ担う
///   - 部位スロット UI は PlayerSystemHub.Instance.EquipSystem.GetPartSlot() で取得
///   - ModuleInfoPanel は ModuleInfoPanel.Instance で参照（SerializeField 不要）
///
/// Canvas にアタッチすること（常にアクティブな親に置く必要がある）
/// </summary>
public class ModuleMenuUI : MonoBehaviour
{
    [Header("メニュー構造")]
    [SerializeField] private GameObject  menuRoot;

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
        menuRoot.SetActive(false); // UIの初期状態だけ Awake で設定
    }

    void Start()
    {
        // PlayerSystemHub.Instance は全 Awake 完了後に確実に存在する
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
            Slot              = PlayerSystemHub.Instance.EquipSystem.GetPartSlot(part),
            InfoPanel         = ModuleInfoPanel.Instance,
            OnClick           = _ => PlayerSystemHub.Instance.EquipSystem.TryUnequip(part),
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
        else if (ModuleInfoPanel.Instance != null)
            ModuleInfoPanel.Instance.Hide();
    }
}
