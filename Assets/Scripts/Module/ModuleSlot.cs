using R3;

/// <summary>
/// モジュールを1つ保持するスロット。インベントリ・部位スロットの両方に使用する。
///
/// 設計原則:
///   - ただの「箱」に徹する。ロジックは持たない
///   - Set / Remove でのみ状態が変化し、変化時は Changed ストリームを流す
///   - 互換性判定・装備処理は EquipSystem が行う
///   - 購読者は .Changed.Subscribe() で変化を受け取り、AddTo(this) で自動管理する
/// </summary>
public class ModuleSlot
{
    /// <summary>このスロットが保持しているモジュール（空なら null）</summary>
    public Module Module { get; private set; }

    public bool IsEmpty   => Module == null;
    public bool HasModule => Module != null;

    private readonly Subject<Unit> _changed = new();

    /// <summary>モジュールが変化した際に流れるストリーム（R3）</summary>
    public Observable<Unit> Changed => _changed;

    // -------------------------------------------------------

    /// <summary>モジュールをこのスロットにセットする</summary>
    public void Set(Module module)
    {
        Module = module;
        _changed.OnNext(Unit.Default);
    }

    /// <summary>スロットからモジュールを取り出して返す。スロットは空になる。</summary>
    public Module Remove()
    {
        var m = Module;
        Module = null;
        _changed.OnNext(Unit.Default);
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
