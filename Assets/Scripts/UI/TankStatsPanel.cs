using LitMotion;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// モジュールメニューの左パネル。
/// 戦車の現在ステータスをベース値＋モジュールボーナスの形式で表示する。
///
/// 表示形式:
///   ボーナスなし → "3"
///   ボーナスあり → "3  (+1)"
///
/// PlayerStats / StatsSystem は PlayerSystemHub.Instance から自動取得するため
/// SerializeField 設定不要。ステータス変化時に LitMotion でポップアニメを再生する。
///
/// シーン構成（このコンポーネントをアタッチした GameObject 内に以下を配置）:
///   Panel_Left
///   ├── Row_HP          → StatRowUI (iconImage, valueText)
///   ├── Row_MoveSpeed   → StatRowUI
///   ├── Row_TurnSpeed   → StatRowUI
///   ├── Row_FireCD      → StatRowUI
///   ├── Row_BulletSpeed → StatRowUI
///   └── Row_MaxAmmo     → StatRowUI
/// </summary>
public class TankStatsPanel : MonoBehaviour
{
    [Header("各ステータス行")]
    [SerializeField] private StatRowUI hpRow;
    [SerializeField] private StatRowUI moveSpeedRow;
    [SerializeField] private StatRowUI turnSpeedRow;
    [SerializeField] private StatRowUI fireCooldownRow;
    [SerializeField] private StatRowUI bulletSpeedRow;
    [SerializeField] private StatRowUI maxAmmoRow;

    // -------------------------------------------------------

    void Start()
    {
        // BonusChanged ストリームを Subscribe — ボーナス変化時に全行を更新
        PlayerSystemHub.Instance.StatsSystem.BonusChanged
            .Subscribe(_ => Refresh())
            .AddTo(this);

        Refresh();
    }

    void OnEnable()
    {
        // パネルが再表示された時に最新値に同期
        if (PlayerSystemHub.Instance != null) Refresh();
    }

    // -------------------------------------------------------

    private void Refresh()
    {
        var bonus = PlayerSystemHub.Instance.StatsSystem.CurrentBonus;
        var ps    = PlayerSystemHub.Instance.PlayerStats;

        SetRow(hpRow,           ps.MaxHp,           bonus.hp);
        SetRow(moveSpeedRow,    ps.MoveSpeed,        bonus.moveSpeed);
        SetRow(turnSpeedRow,    ps.TurnSpeed,        bonus.turnSpeed);
        SetRow(fireCooldownRow, ps.FireCooldown,     bonus.fireCooldown);
        SetRow(bulletSpeedRow,  ps.BulletSpeed,      bonus.bulletSpeed);
        SetRow(maxAmmoRow,      ps.MaxAmmo,          bonus.maxAmmo);
    }

    /// <summary>float 値の行を更新。テキストが変わった場合だけポップアニメを再生。</summary>
    private static void SetRow(StatRowUI row, float total, float bonus)
    {
        if (row == null || row.valueText == null) return;

        string text = bonus > 0f
            ? $"{total:0.##}  <color=#7ED321>(+{bonus:0.##})</color>"
            : $"{total:0.##}";

        if (row.valueText.text == text) return;
        row.valueText.text = text;
        UIAnimations.Pop(row.valueText.transform, 0.15f);
    }

    /// <summary>int 値の行を更新。テキストが変わった場合だけポップアニメを再生。</summary>
    private static void SetRow(StatRowUI row, int total, int bonus)
    {
        if (row == null || row.valueText == null) return;

        string text = bonus > 0
            ? $"{total}  <color=#7ED321>(+{bonus})</color>"
            : $"{total}";

        if (row.valueText.text == text) return;
        row.valueText.text = text;
        UIAnimations.Pop(row.valueText.transform, 0.15f);
    }

    // -------------------------------------------------------

    /// <summary>
    /// ステータス1行分の UI データ。
    /// Inspector で各フィールドに UI 部品をドラッグして設定する。
    /// </summary>
    [System.Serializable]
    public class StatRowUI
    {
        [Tooltip("ステータスのアイコン画像（任意）")]
        public Image icon;

        [Tooltip("値を表示する TextMeshPro テキスト")]
        public TextMeshProUGUI valueText;
    }
}
