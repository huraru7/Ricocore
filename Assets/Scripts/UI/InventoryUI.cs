using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Inventory セクションのコントローラ。
///
/// 責務:
///   - TankModuleManager のインベントリスロットを ModuleSlotUI と 1:1 で初期化する
///   - タブボタンでスロット種別フィルタを切り替える
///   - フィルタ変更やモジュール変化に応じて表示/非表示を更新する
///   - ModuleMenuUI から ResetFilter() を受け取って ALL タブに戻す
///
/// シーン構成:
///   Inventory (このコンポーネントをアタッチ)
///   ├── TabBar
///   │   ├── Tab_All    (Button)
///   │   ├── Tab_Turret (Button)
///   │   ├── Tab_Engine (Button)
///   │   ├── Tab_Right  (Button)
///   │   └── Tab_Left   (Button)
///   └── SlotContainer  (GridLayoutGroup 推奨)
///       ├── SlotUI_0   (ModuleSlotUI)
///       ├── SlotUI_1
///       ...
///       └── SlotUI_9
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private TankModuleManager moduleManager;
    [SerializeField] private ModuleInfoPanel   infoPanel;

    [Header("タブ（ALL から順に設定）")]
    [SerializeField] private Button[]   tabButtons;   // ALL / 砲塔 / エンジン / 右 / 左
    [SerializeField] private TankSlot[] tabSlots;     // None / Turret / Engine / Right / Left
    [SerializeField] private Color      tabActiveColor   = Color.white;
    [SerializeField] private Color      tabInactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("インベントリスロット UI（TankModuleManager の inventorySize と同数）")]
    [SerializeField] private ModuleSlotUI[] slotUIs;

    private TankSlot activeFilter = TankSlot.None;

    // 部位ごとの色（タブボタンの色分けに使用）
    private static Color SlotColor(TankSlot slot) => slot switch
    {
        TankSlot.Turret           => new Color(0.29f, 0.56f, 0.85f), // 青
        TankSlot.Engine           => new Color(0.96f, 0.65f, 0.14f), // 黄
        TankSlot.RightCaterpillar => new Color(0.82f, 0.01f, 0.11f), // 赤
        TankSlot.LeftCaterpillar  => new Color(0.49f, 0.82f, 0.13f), // 緑
        _                         => Color.white
    };

    // -------------------------------------------------------

    void Awake()
    {
        InitializeSlots();
        InitializeTabs();
    }

    void OnEnable()
    {
        if (moduleManager != null)
            moduleManager.OnModuleChanged += RefreshDisplay;
    }

    void OnDisable()
    {
        if (moduleManager != null)
            moduleManager.OnModuleChanged -= RefreshDisplay;
    }

    // -------------------------------------------------------
    // 公開 API

    /// <summary>
    /// ModuleMenuUI から呼び出してフィルタを ALL に戻す。
    /// メニューを開いたタイミングで呼ぶことを想定している。
    /// </summary>
    public void ResetFilter() => SetFilter(TankSlot.None);

    // -------------------------------------------------------
    // 内部処理

    private void InitializeSlots()
    {
        int count = Mathf.Min(slotUIs.Length, moduleManager.InventorySlots.Length);

        if (slotUIs.Length != moduleManager.InventorySlots.Length)
        {
            Debug.LogWarning($"[InventoryUI] SlotUI 数 ({slotUIs.Length}) と " +
                             $"InventorySlots 数 ({moduleManager.InventorySlots.Length}) が異なります。");
        }

        for (int i = 0; i < count; i++)
        {
            var config = new ModuleSlotUIConfig
            {
                Slot              = moduleManager.InventorySlots[i],
                InfoPanel         = infoPanel,
                OnClick           = slot => moduleManager.AutoEquip(slot),
                Label             = (i + 1).ToString(),
                ActionButtonLabel = "装着",
                SlotColor         = Color.clear  // インベントリ枠はデフォルト色
            };
            slotUIs[i].Initialize(config);
        }
    }

    private void InitializeTabs()
    {
        if (tabButtons == null || tabSlots == null) return;

        int n = Mathf.Min(tabButtons.Length, tabSlots.Length);
        for (int i = 0; i < n; i++)
        {
            var ts = tabSlots[i]; // ラムダキャプチャ用にローカル変数化
            tabButtons[i].onClick.AddListener(() => SetFilter(ts));
        }

        UpdateTabColors();
    }

    private void SetFilter(TankSlot filter)
    {
        activeFilter = filter;

        if (infoPanel != null)
            infoPanel.Hide();

        RefreshDisplay();
        UpdateTabColors();
    }

    private void RefreshDisplay()
    {
        int count = Mathf.Min(slotUIs.Length, moduleManager.InventorySlots.Length);
        for (int i = 0; i < count; i++)
        {
            var slot  = moduleManager.InventorySlots[i];
            bool show = activeFilter == TankSlot.None
                     || (slot.HasModule && slot.Module.IsCompatible(activeFilter));
            slotUIs[i].gameObject.SetActive(show);
        }
    }

    private void UpdateTabColors()
    {
        if (tabButtons == null || tabSlots == null) return;

        int n = Mathf.Min(tabButtons.Length, tabSlots.Length);
        for (int i = 0; i < n; i++)
        {
            var img = tabButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = tabSlots[i] == activeFilter ? tabActiveColor : tabInactiveColor;
        }
    }
}
