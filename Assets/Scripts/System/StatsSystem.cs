using R3;
using UnityEngine;

/// <summary>
/// 装備中モジュールからのステータスボーナスを再計算するシステム。
/// Player GameObject にアタッチする。
///
/// 責務:
///   - EquipSystem の PartSlots を R3 で監視し、変化時に Recalculate を呼ぶ
///   - 合計ボーナスを PlayerStats へ反映する
///   - BonusChanged ストリームを流して TankStatsPanel 等に通知する
/// </summary>
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(EquipSystem))]
public class StatsSystem : MonoBehaviour
{
    private PlayerStats playerStats;
    private EquipSystem equipSystem;

    /// <summary>現在の装備ボーナス合計（TankStatsPanel 等で参照）</summary>
    public StatBonus CurrentBonus { get; private set; }

    private readonly Subject<Unit> _bonusChanged = new();

    /// <summary>ボーナスが再計算された際に流れるストリーム（R3）</summary>
    public Observable<Unit> BonusChanged => _bonusChanged;

    // -------------------------------------------------------

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        equipSystem = GetComponent<EquipSystem>();
    }

    void Start()
    {
        // EquipSystem.Awake() が完了してから購読する（Start で確実に担保）
        // AddTo(this) で MonoBehaviour の破棄時に自動解除
        foreach (var slot in equipSystem.PartSlots)
            slot.Changed.Subscribe(_ => Recalculate()).AddTo(this);

        Recalculate();
    }

    void OnDestroy()
    {
        _bonusChanged.Dispose();
        // スロット購読は AddTo(this) で自動解除済み
    }

    // -------------------------------------------------------

    private void Recalculate()
    {
        var total = StatBonus.Zero;

        foreach (var slot in equipSystem.PartSlots)
        {
            if (slot.IsEmpty) continue;
            var b = slot.Module.GetTotalStatBonus();
            total.moveSpeed    += b.moveSpeed;
            total.turnSpeed    += b.turnSpeed;
            total.fireCooldown += b.fireCooldown;
            total.bulletSpeed  += b.bulletSpeed;
            total.hp           += b.hp;
            total.maxAmmo      += b.maxAmmo;
        }

        CurrentBonus = total;
        playerStats.SetModuleBonus(total);
        _bonusChanged.OnNext(Unit.Default);
    }
}
