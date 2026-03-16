using UnityEngine;

/// <summary>
/// モジュールの特殊効果の基底クラス。
/// 新しい特殊効果はこれを継承した ScriptableObject を1ファイル追加するだけで実装できる。
/// TankModuleManager や ModuleDefinition 本体の変更は不要。
/// </summary>
public abstract class ModuleEffect : ScriptableObject
{
    /// <summary>モジュール装着時に呼ばれる</summary>
    public abstract void OnEquip(TankModuleManager manager);

    /// <summary>モジュール取り外し時に呼ばれる</summary>
    public abstract void OnUnequip(TankModuleManager manager);

    /// <summary>
    /// このエフェクトが StatBonus へ寄与する値を返す。
    /// StatBonus に影響しない純粋な特殊効果は StatBonus.Zero を返せばよい（オーバーライド不要）。
    /// </summary>
    public virtual StatBonus GetStatBonus() => StatBonus.Zero;
}
