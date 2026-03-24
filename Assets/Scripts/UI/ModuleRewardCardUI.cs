using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// レベルアップ選択画面のカード1枚分のUI。
/// LevelUpUI が Initialize() を呼んでモジュール情報とコールバックをセットする。
///
/// シーン構成（このコンポーネントをアタッチした GameObject 内に以下を配置）:
///   Card
///   ├── IconImage    (Image)
///   ├── NameText     (TextMeshProUGUI)
///   ├── SlotsText    (TextMeshProUGUI)  ← 装着可能部位
///   ├── StatsText    (TextMeshProUGUI)  ← ステータスボーナス一覧
///   ├── DescText     (TextMeshProUGUI)
///   └── SelectButton (Button)
/// </summary>
public class ModuleRewardCardUI : MonoBehaviour
{
    [Header("UI 部品")]
    [SerializeField] private Image           iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI slotsText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button          selectButton;

    // -------------------------------------------------------

    /// <summary>
    /// カードにモジュール情報をセットする。
    /// onSelect: このカードが選ばれたときのコールバック
    /// </summary>
    public void Initialize(ModuleDefinition def, System.Action<ModuleDefinition> onSelect)
    {
        if (iconImage != null)      iconImage.sprite    = def.icon;
        if (nameText != null)       nameText.text        = def.moduleName;
        if (slotsText != null)      slotsText.text       = BuildSlotsText(def.compatibleSlots);
        if (statsText != null)      statsText.text       = BuildStatsText(def.GetTotalStatBonus());
        if (descriptionText != null) descriptionText.text = def.description;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelect(def));
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
