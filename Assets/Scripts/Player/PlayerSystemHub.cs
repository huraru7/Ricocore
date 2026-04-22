using UnityEngine;

/// <summary>
/// Player 上の全システムを一元管理するハブ。
/// 各 UI スクリプトはこの static Instance を通じてプレイヤーシステムへアクセスする。
///
/// 使い方:
///   1. Player GameObject にこのコンポーネントを追加
///   2. Inspector の ModuleDatabase にアセットをドラッグ設定（唯一の SO 参照）
///   3. UI 側は PlayerSystemHub.Instance.EquipSystem などでアクセスするだけ
///
/// 設計意図:
///   UI スクリプトが EquipSystem / PlayerStats 等を個別に SerializeField で持つと
///   シーン更新のたびに複数箇所を手動設定し直す必要がある。
///   このハブを経由することで、プレイヤーシステムへの参照をここ1箇所に集約する。
/// </summary>
[DefaultExecutionOrder(-100)] // 全スクリプトの Awake より先に実行して Instance を設定
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(EquipSystem))]
[RequireComponent(typeof(StatsSystem))]
[RequireComponent(typeof(ExperienceSystem))]
[RequireComponent(typeof(TankModuleManager))]
public class PlayerSystemHub : MonoBehaviour
{
    // -------------------------------------------------------
    // static Instance（UI からアクセスするエントリポイント）

    /// <summary>シーン内の唯一のプレイヤーシステムへのエントリポイント</summary>
    public static PlayerSystemHub Instance { get; private set; }

    // -------------------------------------------------------
    // Player システム（GetComponent で自動収集 → SerializeField 不要）

    public PlayerStats       PlayerStats   { get; private set; }
    public EquipSystem       EquipSystem   { get; private set; }
    public StatsSystem       StatsSystem   { get; private set; }
    public ExperienceSystem  ExpSystem     { get; private set; }
    public TankModuleManager ModuleManager { get; private set; }

    /// <summary>PlayerStats が所有するリアクティブステート（UI の Subscribe エントリポイント）</summary>
    public PlayerState PlayerState => PlayerStats.State;

    // -------------------------------------------------------
    // ScriptableObject 参照（全 UI で共有するため唯一ここだけで設定）

    [SerializeField, Tooltip("レベルアップ報酬の抽選に使用するモジュールデータベース")]
    public ModuleDatabase ModuleDatabase;

    // -------------------------------------------------------

    void Awake()
    {
        Instance      = this;
        PlayerStats   = GetComponent<PlayerStats>();
        EquipSystem   = GetComponent<EquipSystem>();
        StatsSystem   = GetComponent<StatsSystem>();
        ExpSystem     = GetComponent<ExperienceSystem>();
        ModuleManager = GetComponent<TankModuleManager>();

        Debug.Assert(ModuleDatabase != null,
            "[PlayerSystemHub] ModuleDatabase が未設定です。Inspector で設定してください。");
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
