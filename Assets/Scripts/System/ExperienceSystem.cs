using UnityEngine;

/// <summary>
/// プレイヤーの経験値・レベルを管理するシステム。
/// Player GameObject にアタッチする。
///
/// 仕組み:
///   - 敵が死亡すると EnemyStats.OnEnemyKilled が発火し AddExp() が呼ばれる
///   - 必要経験値 = Level * baseExpPerLevel（線形成長）
///   - 必要量を超えるとレベルアップ。PlayerState.Level を更新して LevelUpUI に通知する
/// </summary>
[RequireComponent(typeof(TankModuleManager))]
public class ExperienceSystem : MonoBehaviour
{
    [Header("経験値設定")]
    [SerializeField, Tooltip("レベル N → N+1 に必要な経験値 = N × この値")]
    private int baseExpPerLevel = 100;

    /// <summary>現在のレベル（1始まり）</summary>
    public int Level      { get; private set; } = 1;

    /// <summary>現在の経験値（次レベルまでの蓄積量）</summary>
    public int CurrentExp { get; private set; }

    /// <summary>次のレベルアップに必要な経験値</summary>
    public int ExpToNext  => Level * baseExpPerLevel;

    private PlayerState playerState;

    // -------------------------------------------------------

    void Start()
    {
        // PlayerSystemHub.Instance は全 Awake 完了後に確実に存在する
        playerState = PlayerSystemHub.Instance.PlayerStats.State;

        // 初期値をステートに書き込む
        playerState.Level.Value    = Level;
        playerState.Exp.Value      = CurrentExp;
        playerState.ExpToNext.Value = ExpToNext;
    }

    // EnemyStats への購読は既存のままゲームロジック側で管理
    void OnEnable()  => EnemyStats.OnEnemyKilled += AddExp;
    void OnDisable() => EnemyStats.OnEnemyKilled -= AddExp;

    // -------------------------------------------------------

    /// <summary>経験値を加算し、必要に応じてレベルアップ処理を行う。</summary>
    public void AddExp(int amount)
    {
        if (amount <= 0) return;

        CurrentExp += amount;

        // 必要経験値を超えている間レベルアップを繰り返す
        while (CurrentExp >= ExpToNext)
        {
            CurrentExp -= ExpToNext;
            Level++;
            // PlayerState を更新 → LevelUpUI の Subscribe が反応する
            playerState.Level.Value    = Level;
            playerState.ExpToNext.Value = ExpToNext; // Level 変化で ExpToNext も変わる
        }

        playerState.Exp.Value = CurrentExp;
    }
}
