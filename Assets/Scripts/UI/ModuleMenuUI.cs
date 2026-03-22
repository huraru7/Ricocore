using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Eキーで開閉する「戦車内部情報」メニューのコントローラ。
///
/// 設計:
///   - 部位スロット UI (4つ) と インベントリスロット UI (inventorySize 個) を Awake で初期化
///   - 各 ModuleSlotUI は ModuleSlot.OnChanged を購読しており、変化時に自動で表示を更新する
///   - タブボタンで インベントリをスロット種別でフィルタリングできる
///
/// シーン構成:
///   ModuleMenuRoot (このコンポーネントをアタッチ, 初期非表示)
///   ├── Panel_Left        → TankStatsPanel
///   ├── Panel_Center
///   │   ├── Image_Tank
///   │   ├── SlotUI_Turret        → ModuleSlotUI
///   │   ├── SlotUI_Engine        → ModuleSlotUI
///   │   ├── SlotUI_Right         → ModuleSlotUI
///   │   └── SlotUI_Left          → ModuleSlotUI
///   ├── Panel_Right       → ModuleInfoPanel
///   └── Panel_Bottom
///       ├── TabBar        → タブボタン × 5
///       └── Panel_Inventory
///           └── SlotUI_Inv_0 〜 N → ModuleSlotUI
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

    [Header("タブフィルタリング")]
    [SerializeField] private Button[]   tabButtons;   // ALL, 砲塔, エンジン, 右, 左 の順
    [SerializeField] private TankSlot[] tabSlots;     // None, Turret, Engine, Right, Left の順
    [SerializeField] private Color      tabActiveColor   = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color      tabInactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private bool      isOpen;
    private TankSlot  activeFilter = TankSlot.None;

    // 部位ごとの色（参考画像に合わせた配色）
    private static readonly Color ColorTurret           = new Color(0.29f, 0.56f, 0.85f); // 青
    private static readonly Color ColorEngine           = new Color(0.96f, 0.65f, 0.14f); // 黄
    private static readonly Color ColorRightCaterpillar = new Color(0.82f, 0.01f, 0.11f); // 赤
    private static readonly Color ColorLeftCaterpillar  = new Color(0.49f, 0.82f, 0.13f); // 緑

    // -------------------------------------------------------

    void Awake()
    {
        menuRoot.SetActive(false);

        // 部位スロット UI を初期化（クリック → 取り外してインベントリへ）
        slotTurret.Initialize(
            moduleManager.PartSlots[TankSlot.Turret], infoPanel,
            _ => moduleManager.UnequipFromPart(TankSlot.Turret),
            "砲塔", "取り外し", ColorTurret);

        slotEngine.Initialize(
            moduleManager.PartSlots[TankSlot.Engine], infoPanel,
            _ => moduleManager.UnequipFromPart(TankSlot.Engine),
            "エンジン", "取り外し", ColorEngine);

        slotRightCaterpillar.Initialize(
            moduleManager.PartSlots[TankSlot.RightCaterpillar], infoPanel,
            _ => moduleManager.UnequipFromPart(TankSlot.RightCaterpillar),
            "右キャタピラー", "取り外し", ColorRightCaterpillar);

        slotLeftCaterpillar.Initialize(
            moduleManager.PartSlots[TankSlot.LeftCaterpillar], infoPanel,
            _ => moduleManager.UnequipFromPart(TankSlot.LeftCaterpillar),
            "左キャタピラー", "取り外し", ColorLeftCaterpillar);

        // インベントリスロット UI を初期化
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
            uiSlot.Initialize(invSlot, infoPanel,
                slot => moduleManager.AutoEquip(slot),
                (i + 1).ToString(), "装着");
        }

        // タブボタンのクリックを登録
        if (tabButtons != null && tabSlots != null)
        {
            int tabCount = System.Math.Min(tabButtons.Length, tabSlots.Length);
            for (int i = 0; i < tabCount; i++)
            {
                var slot = tabSlots[i]; // ラムダキャプチャ用にローカル変数化
                tabButtons[i].onClick.AddListener(() => SetFilter(slot));
            }
        }

        UpdateTabColors();
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            ToggleMenu();
    }

    // -------------------------------------------------------

    private void ToggleMenu()
    {
        isOpen = !isOpen;
        menuRoot.SetActive(isOpen);

        if (!isOpen)
            infoPanel.Hide();
    }

    /// <summary>
    /// 指定スロット種別でインベントリをフィルタリングする。
    /// TankSlot.None で全表示。
    /// </summary>
    private void SetFilter(TankSlot filter)
    {
        activeFilter = filter;
        infoPanel.Hide();

        int count = System.Math.Min(inventorySlotUIs.Length, moduleManager.InventorySlots.Length);
        for (int i = 0; i < count; i++)
        {
            var slot  = moduleManager.InventorySlots[i];
            bool show = filter == TankSlot.None
                     || (slot.HasModule && slot.Module.IsCompatible(filter));
            inventorySlotUIs[i].gameObject.SetActive(show);
        }

        UpdateTabColors();
    }

    private void UpdateTabColors()
    {
        if (tabButtons == null || tabSlots == null) return;
        int tabCount = System.Math.Min(tabButtons.Length, tabSlots.Length);
        for (int i = 0; i < tabCount; i++)
        {
            var img = tabButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = tabSlots[i] == activeFilter ? tabActiveColor : tabInactiveColor;
        }
    }

    // -------------------------------------------------------

    /// <summary>スロット種別に対応した色を返すユーティリティ</summary>
    public static Color SlotColor(TankSlot slot) => slot switch
    {
        TankSlot.Turret           => ColorTurret,
        TankSlot.Engine           => ColorEngine,
        TankSlot.RightCaterpillar => ColorRightCaterpillar,
        TankSlot.LeftCaterpillar  => ColorLeftCaterpillar,
        _                         => Color.white
    };
}
