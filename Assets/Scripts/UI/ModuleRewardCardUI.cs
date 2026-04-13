using System.Text;
using UnityEngine.UIElements;

/// <summary>
/// レベルアップ選択画面のカード 1 枚分のコントローラ（UI Toolkit 版）。
/// LevelUpUI が VisualTreeAsset.CloneTree() でインスタンスを作り、
/// このクラスのコンストラクタに渡して使う。
///
/// UXML: Assets/UI/UXML/ModuleRewardCard.uxml
/// USS:  Assets/UI/USS/Common.uss  (.card / .card__* クラス)
///
/// uGUI 版との主な違い:
///   MonoBehaviour ではなく plain class — GameObject / Prefab 不要
///   SerializeField 配線 → コンストラクタで Q<T>() 取得
///   Image iconImage → VisualElement "icon-image" (background-image)
///   Button.onClick  → Button.clicked
/// </summary>
public class ModuleRewardCardUI
{
    // ---- VisualElement 参照 ----
    public readonly VisualElement Root;   // LevelUpUI がコンテナに追加する対象

    private readonly VisualElement iconImage;
    private readonly Label         nameText;
    private readonly Label         slotsText;
    private readonly Label         statsText;
    private readonly Label         descriptionText;
    private readonly Button        selectButton;

    // clicked コールバック管理（-= で解除するためフィールドで保持）
    private System.Action _onSelectClicked;

    // -------------------------------------------------------

    /// <summary>
    /// CloneTree() で生成した VisualElement を受け取り、子要素を取得する。
    /// </summary>
    public ModuleRewardCardUI(VisualElement cardRoot)
    {
        Root = cardRoot;

        iconImage       = cardRoot.Q<VisualElement>("icon-image");
        nameText        = cardRoot.Q<Label>("name-text");
        slotsText       = cardRoot.Q<Label>("slots-text");
        statsText       = cardRoot.Q<Label>("stats-text");
        descriptionText = cardRoot.Q<Label>("desc-text");
        selectButton    = cardRoot.Q<Button>("select-button");
    }

    // -------------------------------------------------------

    /// <summary>
    /// カードにモジュール情報をセットし、選択コールバックを登録する。
    /// </summary>
    public void Initialize(ModuleDefinition def, System.Action<ModuleDefinition> onSelect)
    {
        if (iconImage != null && def.icon != null)
            iconImage.style.backgroundImage = new StyleBackground(def.icon);

        if (nameText != null)        nameText.text        = def.moduleName;
        if (slotsText != null)       slotsText.text       = BuildSlotsText(def.compatibleSlots);
        if (statsText != null)       statsText.text       = BuildStatsText(def.GetTotalStatBonus());
        if (descriptionText != null) descriptionText.text = def.description;

        if (selectButton != null)
        {
            selectButton.clicked -= _onSelectClicked;
            _onSelectClicked = () => onSelect(def);
            selectButton.clicked += _onSelectClicked;
        }
    }

    // -------------------------------------------------------

    private static string BuildSlotsText(SlotType[] slots)
    {
        if (slots == null || slots.Length == 0) return "";
        var names = System.Array.ConvertAll(slots, s => s switch
        {
            SlotType.Turret           => "砲塔",
            SlotType.Engine           => "エンジン",
            SlotType.RightCaterpillar => "右キャタピラー",
            SlotType.LeftCaterpillar  => "左キャタピラー",
            _                         => s.ToString()
        });
        return "装着可能: " + string.Join(", ", names);
    }

    private static string BuildStatsText(StatBonus b)
    {
        var sb = new StringBuilder();
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
        sb.AppendLine($"{label}: {(value > 0f ? "+" : "")}{value:0.##}");
    }

    private static void AppendStat(StringBuilder sb, string label, int value)
    {
        if (value == 0) return;
        sb.AppendLine($"{label}: {(value > 0 ? "+" : "")}{value}");
    }
}
