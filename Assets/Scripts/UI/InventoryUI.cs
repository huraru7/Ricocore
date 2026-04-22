using R3;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Inventory セクションのコントローラ（UI Toolkit 版）。
///
/// UXML: Assets/UI/UXML/Inventory.uxml
/// USS:  Assets/UI/USS/Common.uss
///
/// 責務:
///   - EquipSystem のインベントリスロットを ModuleSlotUI と 1:1 で初期化する
///   - タブボタンでスロット種別フィルタを切り替える
///   - フィルタ変更やモジュール変化に応じて表示/非表示を更新する
///   - ModuleMenuUI から ResetFilter() を受け取って ALL タブに戻す
///
/// uGUI 版との主な違い:
///   [SerializeField] Button[] tabButtons → UXML から Q<Button> で取得
///   [SerializeField] ModuleSlotUI[] slotUIs → CloneTree() で動的生成
///   Image.color でタブ色変更 → EnableInClassList("tab--active") で切り替え
///   slotUIs[i].gameObject.SetActive → slot.Root.style.display
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class InventoryUI : MonoBehaviour
{
    [Header("スロットテンプレート（ModuleSlot.uxml を設定）")]
    [SerializeField] private VisualTreeAsset slotTemplate;

    // ---- VisualElement 参照 ----
    private Button[]        tabButtons;   // ALL / 砲塔 / エンジン / 右 / 左
    private VisualElement   slotContainer;

    // タブに対応する SlotType（tabButtons と同じ順）
    private static readonly SlotType[] TabSlots =
    {
        SlotType.None,
        SlotType.Turret,
        SlotType.Engine,
        SlotType.RightCaterpillar,
        SlotType.LeftCaterpillar,
    };

    private ModuleSlotUI[] slotUIs;
    private SlotType activeFilter = SlotType.None;

    // -------------------------------------------------------

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        tabButtons = new Button[]
        {
            root.Q<Button>("tab-all"),
            root.Q<Button>("tab-turret"),
            root.Q<Button>("tab-engine"),
            root.Q<Button>("tab-right"),
            root.Q<Button>("tab-left"),
        };

        slotContainer = root.Q<VisualElement>("slot-container");
    }

    void Start()
    {
        InitializeSlots();
        InitializeTabs();

        var eq = PlayerSystemHub.Instance.EquipSystem;

        // 全スロット（インベントリ + 部位）の変化を1ストリームに統合して購読
        var allChanged = eq.InventorySlots
            .Concat(eq.PartSlots.Cast<ModuleSlot>())
            .Select(s => s.Changed)
            .ToArray();

        Observable.Merge(allChanged)
            .Subscribe(_ => RefreshDisplay())
            .AddTo(this);

        RefreshDisplay();
    }

    void OnEnable()
    {
        if (PlayerSystemHub.Instance != null) RefreshDisplay();
    }

    // -------------------------------------------------------
    // 公開 API

    /// <summary>ModuleMenuUI から呼び出してパネルの表示/非表示を切り替える。</summary>
    public void SetVisible(bool visible)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary>ModuleMenuUI から呼び出してフィルタを ALL に戻す。</summary>
    public void ResetFilter() => SetFilter(SlotType.None);

    // -------------------------------------------------------
    // 内部処理

    private void InitializeSlots()
    {
        var eq    = PlayerSystemHub.Instance.EquipSystem;
        int count = eq.InventorySlots.Length;

        slotUIs = new ModuleSlotUI[count];

        for (int i = 0; i < count; i++)
        {
            var instance = slotTemplate.CloneTree();
            slotContainer.Add(instance);
            slotUIs[i] = new ModuleSlotUI(instance);

            slotUIs[i].Initialize(new ModuleSlotUIConfig
            {
                Slot              = eq.InventorySlots[i],
                InfoPanel         = ModuleInfoPanel.Instance,
                OnClick           = slot => eq.TryEquip(slot),
                Label             = (i + 1).ToString(),
                ActionButtonLabel = "装着",
                SlotColor         = Color.clear,
            });
        }
    }

    private void InitializeTabs()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] == null) continue;
            var ts = TabSlots[i]; // ラムダキャプチャ用
            tabButtons[i].clicked += () => SetFilter(ts);
        }

        UpdateTabStyles();
    }

    private void SetFilter(SlotType filter)
    {
        activeFilter = filter;
        ModuleInfoPanel.Instance?.Hide();
        RefreshDisplay();
        UpdateTabStyles();
    }

    private void RefreshDisplay()
    {
        if (slotUIs == null) return;

        var eq = PlayerSystemHub.Instance.EquipSystem;
        int count = Mathf.Min(slotUIs.Length, eq.InventorySlots.Length);

        for (int i = 0; i < count; i++)
        {
            var slot = eq.InventorySlots[i];
            bool show = activeFilter == SlotType.None
                     || (slot.HasModule && slot.Module.IsCompatible(activeFilter));

            slotUIs[i].Root.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void UpdateTabStyles()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] == null) continue;
            tabButtons[i].EnableInClassList("tab--active", TabSlots[i] == activeFilter);
        }
    }
}
