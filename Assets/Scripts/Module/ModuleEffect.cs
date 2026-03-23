using UnityEngine;

/// <summary>
/// モジュールの特殊効果の基底クラス。
/// 新しい特殊効果はこれを継承した ScriptableObject を1ファイル追加するだけで実装できる。
/// </summary>
public abstract class ModuleEffect : ScriptableObject
{
    /// <summary>モジュールが部位スロットへ装着された時に呼ばれる</summary>
    public abstract void OnEquip(EquipSystem equipSystem);

    /// <summary>モジュールが部位スロットから取り外された時に呼ばれる</summary>
    public abstract void OnUnequip(EquipSystem equipSystem);

    /// <summary>
    /// このエフェクトが StatBonus へ寄与する値を返す。
    /// StatBonus に影響しない純粋な特殊効果は StatBonus.Zero を返せばよい（オーバーライド不要）。
    /// </summary>
    public virtual StatBonus GetStatBonus() => StatBonus.Zero;
}
