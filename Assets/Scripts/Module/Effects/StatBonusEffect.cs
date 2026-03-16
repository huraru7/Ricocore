using UnityEngine;

/// <summary>
/// StatBonus をそのまま付与する最もシンプルな ModuleEffect 実装。
/// 移動速度や弾薬数などを上昇させるだけのモジュールはこれを使う。
/// </summary>
[CreateAssetMenu(fileName = "StatBonusEffect", menuName = "TankGame/Module/Effects/StatBonus")]
public class StatBonusEffect : ModuleEffect
{
    [SerializeField] private StatBonus bonus;

    public override void OnEquip(TankModuleManager manager) { }
    public override void OnUnequip(TankModuleManager manager) { }
    public override StatBonus GetStatBonus() => bonus;
}
