using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// スロットにカーソルを合わせた際にモジュール情報を表示するパネル（UI Toolkit 版）。
/// ModuleSlotUI がホバー時に Show(module, onButtonClick, buttonLabel) / Hide() を呼ぶ。
///
/// UXML: Assets/UI/UXML/ModuleInfoPanel.uxml
/// USS:  Assets/UI/USS/Common.uss  (.info-panel / .info-panel__* クラス)
///
/// セットアップ:
///   1. GameObject に UIDocument をアタッチし、ModuleInfoPanel.uxml を設定
///   2. 同 GameObject にこのコンポーネントをアタッチ
///
/// uGUI 版との対応:
///   root.SetActive(false/true) → panel.style.display = DisplayStyle.None/Flex
///   SerializeField 配線        → Awake() で Q<T>() 取得
///   static Instance            → そのまま維持（呼び出し側は変更不要）
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class ModuleInfoPanel : MonoBehaviour
{
    /// <summary>シーン内の唯一の ModuleInfoPanel へのアクセス用</summary>
    public static ModuleInfoPanel Instance { get; private set; }

    // ---- VisualElement 参照 ----
    private VisualElement panel;        // パネル全体（表示/非表示の制御対象）
    private VisualElement iconImage;
    private Label         nameText;
    private Label         descriptionText;
    private Label         statsText;
    private Label         slotsText;
    private Button        actionButton;

    private readonly StringBuilder sb = new StringBuilder();

    // -------------------------------------------------------

    void Awake()
    {
        Instance = this;

        var root = GetComponent<UIDocument>().rootVisualElement;

        panel           = root.Q<VisualElement>("info-panel-root");
        iconImage       = root.Q<VisualElement>("icon-image");
        nameText        = root.Q<Label>("name-text");
        descriptionText = root.Q<Label>("desc-text");
        statsText       = root.Q<Label>("stats-text");
        slotsText       = root.Q<Label>("slots-text");
        actionButton    = root.Q<Button>("action-button");

        Hide();
    }

    // -------------------------------------------------------

    /// <summary>
    /// 指定モジュールの情報をパネルに表示する。
    /// </summary>
    /// <param name="module">表示するモジュール</param>
    /// <param name="onButtonClick">ボタン押下時のコールバック（null でボタン非表示）</param>
    /// <param name="buttonLabel">"装着" or "取り外し"</param>
    public void Show(Module module, System.Action onButtonClick, string buttonLabel)
    {
        panel.style.display = DisplayStyle.Flex;

        if (iconImage != null && module.Icon != null)
            iconImage.style.backgroundImage = new StyleBackground(module.Icon);

        if (nameText != null)        nameText.text        = module.Name;
        if (descriptionText != null) descriptionText.text = module.Description;
        if (statsText != null)       statsText.text       = BuildStatsText(module);
        if (slotsText != null)       slotsText.text       = BuildCompatibleSlotsText(module);

        if (actionButton != null)
        {
            if (onButtonClick != null)
            {
                actionButton.style.display = DisplayStyle.Flex;
                actionButton.text = buttonLabel;

                // clicked は重複登録しないよう一度解除してから登録
                actionButton.clicked -= _currentButtonCallback;
                _currentButtonCallback = onButtonClick;
                actionButton.clicked += _currentButtonCallback;
            }
            else
            {
                actionButton.style.display = DisplayStyle.None;
            }
        }
    }

    /// <summary>パネルを非表示にする</summary>
    public void Hide()
    {
        if (panel != null)
            panel.style.display = DisplayStyle.None;
    }

    // -------------------------------------------------------
    // clicked コールバック管理（-= で解除するためフィールドで保持）

    private System.Action _currentButtonCallback;

    // -------------------------------------------------------

    private string BuildStatsText(Module module)
    {
        var b = module.GetTotalStatBonus();
        sb.Clear();

        AppendStat(sb, "移動速度",         b.moveSpeed);
        AppendStat(sb, "旋回速度",         b.turnSpeed);
        AppendStat(sb, "射撃クールダウン", b.fireCooldown);
        AppendStat(sb, "弾速",             b.bulletSpeed);
        AppendStat(sb, "HP",               b.hp);
        AppendStat(sb, "最大弾数",         b.maxAmmo);

        return sb.Length > 0 ? sb.ToString().TrimEnd() : "効果なし";
    }

    private static void AppendStat(StringBuilder sb, string label, float value)
    {
        if (value == 0f) return;
        sb.AppendLine($"{label}: {(value > 0f ? "+" : "")}{value}");
    }

    private static void AppendStat(StringBuilder sb, string label, int value)
    {
        if (value == 0) return;
        sb.AppendLine($"{label}: {(value > 0 ? "+" : "")}{value}");
    }

    private static string BuildCompatibleSlotsText(Module module)
    {
        var slots = module.CompatibleSlots;
        if (slots == null || slots.Length == 0) return "";

        var names = System.Array.ConvertAll(slots, GetSlotDisplayName);
        return "装着可能: " + string.Join(", ", names);
    }

    private static string GetSlotDisplayName(SlotType slot) => slot switch
    {
        SlotType.Turret           => "砲塔",
        SlotType.Engine           => "エンジン",
        SlotType.RightCaterpillar => "右キャタピラー",
        SlotType.LeftCaterpillar  => "左キャタピラー",
        _                         => slot.ToString()
    };
}
