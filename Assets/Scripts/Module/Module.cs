using UnityEngine;

/// <summary>
/// モジュールのランタイムインスタンス。
/// ModuleDefinition (ScriptableObject) はあくまで元データ（テンプレート）であり、
/// プレイヤーがモジュールを獲得した際にこのクラスのインスタンスが生成される。
/// 将来的にはここに強化レベルや個体差パラメータを追加できる。
/// </summary>
public class Module
{
    /// <summary>元データ（テンプレート）への参照</summary>
    public ModuleDefinition Definition { get; }

    // ---- ModuleDefinition へのショートカット ----
    public string     Name           => Definition.moduleName;
    public string     Description    => Definition.description;
    public Sprite     Icon           => Definition.icon;
    public TankSlot[] CompatibleSlots => Definition.compatibleSlots;

    public Module(ModuleDefinition definition)
    {
        Definition = definition;
    }

    /// <summary>このモジュールの全 StatBonus を集計して返す</summary>
    public StatBonus GetTotalStatBonus() => Definition.GetTotalStatBonus();

    /// <summary>指定スロットに装着可能かを返す</summary>
    public bool IsCompatible(TankSlot slot) => Definition.IsCompatible(slot);
}
