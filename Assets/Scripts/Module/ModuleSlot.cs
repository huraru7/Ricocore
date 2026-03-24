/// <summary>
/// モジュールを1つ保持するスロット。インベントリ・部位スロットの両方に使用する。
///
/// 設計原則:
///   - ただの「箱」に徹する。ロジックは持たない
///   - Set / Remove でのみ状態が変化し、変化時は OnChanged を発火する
///   - 互換性判定・装備処理は EquipSystem が行う
/// </summary>
public class ModuleSlot
{
    /// <summary>このスロットが保持しているモジュール（空なら null）</summary>
    public Module Module { get; private set; }

    public bool IsEmpty   => Module == null;
    public bool HasModule => Module != null;

    /// <summary>モジュールが変化した際に発火するイベント</summary>
    public event System.Action OnChanged;

    // -------------------------------------------------------

    /// <summary>モジュールをこのスロットにセットする</summary>
    public void Set(Module module)
    {
        Module = module;
        OnChanged?.Invoke();
    }

    /// <summary>スロットからモジュールを取り出して返す。スロットは空になる。</summary>
    public Module Remove()
    {
        var m = Module;
        Module = null;
        OnChanged?.Invoke();
        return m;
    }
}

/// <summary>
/// 戦車の部位スロット。
/// ModuleSlot を継承し、どの部位か（SlotType）を持つ。
/// EquipSystem が配列として管理する。
/// </summary>
public class PartSlot : ModuleSlot
{
    /// <summary>この部位スロットの種別（Turret / Engine / RightCaterpillar / LeftCaterpillar）</summary>
    public SlotType SlotType { get; }

    public PartSlot(SlotType slotType)
    {
        SlotType = slotType;
    }
}
