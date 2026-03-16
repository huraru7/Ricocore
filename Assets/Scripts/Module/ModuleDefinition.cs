using UnityEngine;

/// <summary>
/// モジュール1種類のデータを保持する ScriptableObject。
/// Assets > Create > TankGame > Module > ModuleDefinition で作成する。
/// </summary>
[CreateAssetMenu(fileName = "Module_New", menuName = "TankGame/Module/ModuleDefinition")]
public class ModuleDefinition : ScriptableObject
{
    [Header("基本情報")]
    public string moduleName;

    [TextArea(2, 4)]
    public string description;

    public Sprite icon;

    [Header("装着可能スロット")]
    public TankSlot[] compatibleSlots;

    [Header("効果")]
    public StatBonus statBonus;

    [Tooltip("将来の特殊効果。現在は空配列でよい。")]
    public ModuleEffect[] specialEffects;

    // -------------------------------------------------------

    /// <summary>
    /// このモジュールの全 StatBonus を集計して返す。
    /// statBonus フィールドと specialEffects の両方を合算する。
    /// </summary>
    public StatBonus GetTotalStatBonus()
    {
        var total = statBonus;

        if (specialEffects == null) return total;

        foreach (var effect in specialEffects)
        {
            if (effect == null) continue;
            var b = effect.GetStatBonus();
            total.moveSpeed    += b.moveSpeed;
            total.turnSpeed    += b.turnSpeed;
            total.fireCooldown += b.fireCooldown;
            total.bulletSpeed  += b.bulletSpeed;
            total.hp           += b.hp;
            total.maxAmmo      += b.maxAmmo;
        }

        return total;
    }

    /// <summary>指定スロットにこのモジュールが装着可能かを返す</summary>
    public bool IsCompatible(TankSlot slot)
    {
        if (compatibleSlots == null) return false;
        foreach (var s in compatibleSlots)
            if (s == slot) return true;
        return false;
    }
}
