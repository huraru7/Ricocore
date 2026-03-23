using UnityEngine;

/// <summary>
/// 装備中モジュールからのステータスボーナスを再計算するシステム。
/// Player GameObject にアタッチする。
///
/// 責務:
///   - EquipSystem の PartSlots を監視し、変化時に Recalculate を呼ぶ
///   - 合計ボーナスを PlayerStats へ反映する
///   - OnChanged イベントを発火して TankStatsPanel 等に通知する
/// </summary>
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(EquipSystem))]
public class StatsSystem : MonoBehaviour
{
    private PlayerStats playerStats;
    private EquipSystem equipSystem;

    /// <summary>現在の装備ボーナス合計（TankStatsPanel 等で参照）</summary>
    public StatBonus CurrentBonus { get; private set; }

    /// <summary>ボーナスが再計算された際に発火する</summary>
    public event System.Action OnChanged;

    // -------------------------------------------------------

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        equipSystem = GetComponent<EquipSystem>();
    }

    void Start()
    {
        // EquipSystem.Awake() が完了してから購読する（Start で確実に担保）
        foreach (var slot in equipSystem.PartSlots)
            slot.OnChanged += Recalculate;

        Recalculate();
    }

    void OnDestroy()
    {
        if (equipSystem == null) return;
        foreach (var slot in equipSystem.PartSlots)
            slot.OnChanged -= Recalculate;
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
        OnChanged?.Invoke();
    }
}
