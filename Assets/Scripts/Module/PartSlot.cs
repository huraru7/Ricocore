/// <summary>
/// 戦車の部位スロット。
/// ModuleSlot を継承し、どの部位か（SlotType）を持つ。
/// EquipSystem が List として管理する。
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
