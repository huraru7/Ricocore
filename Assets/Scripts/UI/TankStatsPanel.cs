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
/// SerializeField 設定不要。
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

    private bool initialized;

    // -------------------------------------------------------

    void Start()
    {
        PlayerSystemHub.Instance.StatsSystem.OnChanged += Refresh;
        initialized = true;
        Refresh();
    }

    void OnEnable()
    {
        if (initialized) PlayerSystemHub.Instance.StatsSystem.OnChanged += Refresh;
    }

    void OnDisable()
    {
        if (PlayerSystemHub.Instance != null)
            PlayerSystemHub.Instance.StatsSystem.OnChanged -= Refresh;
    }

    // -------------------------------------------------------

    private void Refresh()
    {
        var bonus       = PlayerSystemHub.Instance.StatsSystem.CurrentBonus;
        var playerStats = PlayerSystemHub.Instance.PlayerStats;

        hpRow.Set(playerStats.MaxHp,              bonus.hp);
        moveSpeedRow.Set(playerStats.MoveSpeed,    bonus.moveSpeed);
        turnSpeedRow.Set(playerStats.TurnSpeed,    bonus.turnSpeed);
        fireCooldownRow.Set(playerStats.FireCooldown, bonus.fireCooldown);
        bulletSpeedRow.Set(playerStats.BulletSpeed,   bonus.bulletSpeed);
        maxAmmoRow.Set(playerStats.MaxAmmo,        bonus.maxAmmo);
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

        /// <summary>float 値を設定する（移動速度・弾速など）</summary>
        public void Set(float total, float bonus)
        {
            if (valueText == null) return;
            valueText.text = bonus > 0f
                ? $"{total:0.##}  <color=#7ED321>(+{bonus:0.##})</color>"
                : $"{total:0.##}";
        }

        /// <summary>int 値を設定する（HP・最大弾数など）</summary>
        public void Set(int total, int bonus)
        {
            if (valueText == null) return;
            valueText.text = bonus > 0
                ? $"{total}  <color=#7ED321>(+{bonus})</color>"
                : $"{total}";
        }
    }
}
