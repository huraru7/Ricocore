using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スロットにカーソルを合わせた際にモジュール情報を表示するパネル。
/// ModuleSlotUI がホバー時に Show(module, onButtonClick, buttonLabel) / Hide() を呼ぶ。
/// </summary>
public class ModuleInfoPanel : MonoBehaviour
{
    [Header("パネル本体")]
    [SerializeField] private GameObject root;

    [Header("UI 部品")]
    [SerializeField] private Image           iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI compatibleSlotsText;

    [Header("アクションボタン（装着/取り外し）")]
    [SerializeField] private Button          actionButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;

    private readonly StringBuilder sb = new StringBuilder();

    // -------------------------------------------------------

    void Awake() => root.SetActive(false);

    /// <summary>
    /// 指定モジュールの情報をパネルに表示する。
    /// onButtonClick: ボタン押下時のコールバック（null なら非表示）
    /// buttonLabel: ボタンのラベル（"装着" or "取り外し"）
    /// </summary>
    public void Show(Module module, System.Action onButtonClick, string buttonLabel)
    {
        root.SetActive(true);

        iconImage.sprite         = module.Icon;
        nameText.text            = module.Name;
        descriptionText.text     = module.Description;
        statsText.text           = BuildStatsText(module);
        compatibleSlotsText.text = BuildCompatibleSlotsText(module);

        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            if (onButtonClick != null)
            {
                actionButton.gameObject.SetActive(true);
                actionButton.onClick.AddListener(() => onButtonClick());
                if (actionButtonText != null)
                    actionButtonText.text = buttonLabel;
            }
            else
            {
                actionButton.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>パネルを非表示にする</summary>
    public void Hide() => root.SetActive(false);

    // -------------------------------------------------------

    private string BuildStatsText(Module module)
    {
        var b = module.GetTotalStatBonus();
        sb.Clear();

        AppendStat(sb, "移動速度",           b.moveSpeed);
        AppendStat(sb, "旋回速度",           b.turnSpeed);
        AppendStat(sb, "射撃クールダウン",   b.fireCooldown);
        AppendStat(sb, "弾速",               b.bulletSpeed);
        AppendStat(sb, "HP",                 b.hp);
        AppendStat(sb, "最大弾数",           b.maxAmmo);

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

    private static string GetSlotDisplayName(TankSlot slot) => slot switch
    {
        TankSlot.Turret           => "砲塔",
        TankSlot.Engine           => "エンジン",
        TankSlot.RightCaterpillar => "右キャタピラー",
        TankSlot.LeftCaterpillar  => "左キャタピラー",
        _                         => slot.ToString()
    };
}
