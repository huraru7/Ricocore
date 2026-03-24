using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Inventory セクションのコントローラ。
///
/// 責務:
///   - EquipSystem のインベントリスロットを ModuleSlotUI と 1:1 で初期化する
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
///       ...
///       └── SlotUI_9
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private EquipSystem     equipSystem;
    [SerializeField] private ModuleInfoPanel infoPanel;

    [Header("タブ（ALL から順に設定）")]
    [SerializeField] private Button[]    tabButtons;   // ALL / 砲塔 / エンジン / 右 / 左
    [SerializeField] private SlotType[]  tabSlots;     // None / Turret / Engine / Right / Left
    [SerializeField] private Color       tabActiveColor   = Color.white;
    [SerializeField] private Color       tabInactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("インベントリスロット UI（EquipSystem の inventorySize と同数）")]
    [SerializeField] private ModuleSlotUI[] slotUIs;

    private SlotType activeFilter = SlotType.None;

    // -------------------------------------------------------

    void Awake()
    {
        InitializeSlots();
        InitializeTabs();
    }

    void OnEnable()
    {
        // EquipSystem が PartSlots を変更したとき（装着/取り外し）に再フィルタリング
        if (equipSystem != null)
        {
            foreach (var ps in equipSystem.PartSlots)
                ps.OnChanged += RefreshDisplay;
            foreach (var inv in equipSystem.InventorySlots)
                inv.OnChanged += RefreshDisplay;
        }
    }

    void OnDisable()
    {
        if (equipSystem != null)
        {
            foreach (var ps in equipSystem.PartSlots)
                ps.OnChanged -= RefreshDisplay;
            foreach (var inv in equipSystem.InventorySlots)
                inv.OnChanged -= RefreshDisplay;
        }
    }

    // -------------------------------------------------------
    // 公開 API

    /// <summary>
    /// ModuleMenuUI から呼び出してフィルタを ALL に戻す。
    /// メニューを開いたタイミングで呼ぶことを想定している。
    /// </summary>
    public void ResetFilter() => SetFilter(SlotType.None);

    // -------------------------------------------------------
    // 内部処理

    private void InitializeSlots()
    {
        if (equipSystem == null) return;

        int count = Mathf.Min(slotUIs.Length, equipSystem.InventorySlots.Length);

        if (slotUIs.Length != equipSystem.InventorySlots.Length)
            Debug.LogWarning($"[InventoryUI] SlotUI 数 ({slotUIs.Length}) と InventorySlots 数 ({equipSystem.InventorySlots.Length}) が異なります。");

        for (int i = 0; i < count; i++)
        {
            var config = new ModuleSlotUIConfig
            {
                Slot              = equipSystem.InventorySlots[i],
                InfoPanel         = infoPanel,
                OnClick           = slot => equipSystem.TryEquip(slot),
                Label             = (i + 1).ToString(),
                ActionButtonLabel = "装着",
                SlotColor         = Color.clear
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

    private void SetFilter(SlotType filter)
    {
        activeFilter = filter;

        if (infoPanel != null)
            infoPanel.Hide();

        RefreshDisplay();
        UpdateTabColors();
    }

    private void RefreshDisplay()
    {
        if (equipSystem == null) return;

        int count = Mathf.Min(slotUIs.Length, equipSystem.InventorySlots.Length);
        for (int i = 0; i < count; i++)
        {
            var slot  = equipSystem.InventorySlots[i];
            bool show = activeFilter == SlotType.None
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
