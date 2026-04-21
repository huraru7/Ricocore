using R3;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 戦車の現在ステータスを表示するパネル（UI Toolkit 版）。
///
/// UXML: Assets/UI/UXML/TankStatsPanel.uxml
/// USS:  Assets/UI/USS/Common.uss  (.stats-panel / .stat-row / .stat-row__* クラス)
///
/// 表示形式:
///   ボーナスなし → value: "3"   bonus: 非表示
///   ボーナスあり → value: "3"   bonus: "(+1)"（緑色）
///
/// uGUI 版との主な違い:
///   TextMeshPro RichText <color=#...> → value / bonus を別々の Label に分離
///   SerializeField StatRowUI[]        → Awake() で Q<Label> 取得
///   UIAnimations.Pop(transform)       → UIToolkitAnimations.Pop(Label)
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class TankStatsPanel : MonoBehaviour
{
    // ---- 行ごとの Label ペア ----
    private StatRow hp;
    private StatRow speed;
    private StatRow turn;
    private StatRow fireCd;
    private StatRow bullet;
    private StatRow ammo;

    // -------------------------------------------------------

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        hp     = new StatRow(root, "hp");
        speed  = new StatRow(root, "speed");
        turn   = new StatRow(root, "turn");
        fireCd = new StatRow(root, "firecd");
        bullet = new StatRow(root, "bullet");
        ammo   = new StatRow(root, "ammo");
    }

    void Start()
    {
        PlayerSystemHub.Instance.StatsSystem.BonusChanged
            .Subscribe(_ => Refresh())
            .AddTo(this);

        Refresh();
    }

    void OnEnable()
    {
        if (PlayerSystemHub.Instance != null) Refresh();
    }

    // -------------------------------------------------------

    public void SetVisible(bool visible)
    {
        GetComponent<UIDocument>().rootVisualElement.style.display =
            visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    // -------------------------------------------------------

    private void Refresh()
    {
        var bonus = PlayerSystemHub.Instance.StatsSystem.CurrentBonus;
        var ps    = PlayerSystemHub.Instance.PlayerStats;

        SetRow(hp,     ps.MaxHp,       bonus.hp);
        SetRow(speed,  ps.MoveSpeed,   bonus.moveSpeed);
        SetRow(turn,   ps.TurnSpeed,   bonus.turnSpeed);
        SetRow(fireCd, ps.FireCooldown, bonus.fireCooldown);
        SetRow(bullet, ps.BulletSpeed, bonus.bulletSpeed);
        SetRow(ammo,   ps.MaxAmmo,     bonus.maxAmmo);
    }

    private static void SetRow(StatRow row, float total, float bonus)
    {
        string valueText = $"{total:0.##}";
        string bonusText = bonus > 0f ? $"(+{bonus:0.##})" : "";
        UpdateRow(row, valueText, bonusText);
    }

    private static void SetRow(StatRow row, int total, int bonus)
    {
        string valueText = $"{total}";
        string bonusText = bonus > 0 ? $"(+{bonus})" : "";
        UpdateRow(row, valueText, bonusText);
    }

    private static void UpdateRow(StatRow row, string valueText, string bonusText)
    {
        if (row.ValueLabel == null) return;

        // 値が変わった時だけアニメ＆更新
        if (row.ValueLabel.text != valueText)
        {
            row.ValueLabel.text = valueText;
            UIToolkitAnimations.Pop(row.ValueLabel, 0.15f);
        }

        if (row.BonusLabel != null)
        {
            row.BonusLabel.text = bonusText;
            row.BonusLabel.style.display =
                bonusText.Length > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    // -------------------------------------------------------

    /// <summary>stat-row 1行分の Label ペア。</summary>
    private readonly struct StatRow
    {
        public readonly Label ValueLabel;
        public readonly Label BonusLabel;

        public StatRow(VisualElement root, string baseName)
        {
            ValueLabel = root.Q<Label>($"{baseName}-value");
            BonusLabel = root.Q<Label>($"{baseName}-bonus");
        }
    }
}
