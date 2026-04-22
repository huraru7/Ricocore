using UnityEngine;

/// <summary>
/// モジュールの装備ロジックを一元管理するシステム。
/// Player GameObject にアタッチする。
///
/// 責務:
///   - インベントリスロットと部位スロットの生成・保持
///   - TryEquip / TryUnequip / PlaceInInventory による装備操作
///   - SlotTransfer を用いたスロット間モジュール移動
///   - スロット間移動の判定（互換性チェック）
/// </summary>
[RequireComponent(typeof(TankModuleManager))]
public class EquipSystem : MonoBehaviour
{
    [SerializeField, Tooltip("インベントリスロット数")]
    private int inventorySize = 10;

    /// <summary>インベントリスロット（固定長配列）</summary>
    public ModuleSlot[] InventorySlots { get; private set; }

    /// <summary>部位スロット（Turret / Engine / RightCaterpillar / LeftCaterpillar）</summary>
    public PartSlot[] PartSlots { get; private set; }

    // -------------------------------------------------------

    void Awake()
    {
        // インベントリスロット生成
        InventorySlots = new ModuleSlot[inventorySize];
        for (int i = 0; i < inventorySize; i++)
            InventorySlots[i] = new ModuleSlot();

        // 部位スロット生成（None 以外の実在する部位のみ）
        PartSlots = new PartSlot[]
        {
            new PartSlot(SlotType.Turret),
            new PartSlot(SlotType.Engine),
            new PartSlot(SlotType.RightCaterpillar),
            new PartSlot(SlotType.LeftCaterpillar),
        };
    }

    // ============================================================
    // 公開 API
    // ============================================================

    /// <summary>
    /// インベントリスロットのモジュールを互換部位スロットへ自動装着する。
    /// 空きスロット優先。なければ先頭の互換スロットへ入れ替える。
    /// </summary>
    public bool TryEquip(ModuleSlot inventorySlot)
    {
        if (inventorySlot == null || inventorySlot.IsEmpty) return false;

        var target = FindCompatibleEmptySlot(inventorySlot.Module)
                  ?? FindCompatibleSlot(inventorySlot.Module);
        if (target == null) return false;

        SlotTransfer.Move(inventorySlot, target);
        return true;
    }

    /// <summary>
    /// 指定部位スロットのモジュールを空きインベントリスロットへ取り外す。
    /// インベントリが満杯なら false を返す。
    /// </summary>
    public bool TryUnequip(SlotType part)
    {
        var ps = GetPartSlot(part);
        if (ps == null || ps.IsEmpty) return false;

        var inv = FindEmptyInventorySlot();
        if (inv == null) return false;

        SlotTransfer.MoveIfEmpty(ps, inv);
        return true;
    }

    /// <summary>
    /// 空きインベントリスロットへモジュールを直接配置する。
    /// モジュール獲得時（AcquireModule）から呼ばれる。
    /// </summary>
    public bool PlaceInInventory(Module module)
    {
        var slot = FindEmptyInventorySlot();
        if (slot == null) return false;
        slot.Set(module);
        return true;
    }

    /// <summary>指定種別の部位スロットを取得する</summary>
    public PartSlot GetPartSlot(SlotType type) =>
        System.Array.Find(PartSlots, s => s.SlotType == type);

    // ============================================================
    // 内部処理
    // ============================================================

    private PartSlot FindCompatibleEmptySlot(Module m) =>
        System.Array.Find(PartSlots, s => s.IsEmpty && m.IsCompatible(s.SlotType));

    private PartSlot FindCompatibleSlot(Module m) =>
        System.Array.Find(PartSlots, s => m.IsCompatible(s.SlotType));

    private ModuleSlot FindEmptyInventorySlot() =>
        System.Array.Find(InventorySlots, s => s.IsEmpty);
}

// -------------------------------------------------------

/// <summary>
/// スロット間のモジュール移動を担う静的ユーティリティ。
///
/// Inventory → Part / Part → Inventory / Part → Part
/// すべて同じメソッドで実現できる。
/// </summary>
public static class SlotTransfer
{
    /// <summary>
    /// from のモジュールを to へ移動する。
    /// to に既存モジュールがある場合は from へ押し出す（自動入れ替え）。
    /// </summary>
    public static void Move(ModuleSlot from, ModuleSlot to)
    {
        var moduleToMove = from.Remove();

        if (to.HasModule)
        {
            var displaced = to.Remove();
            from.Set(displaced); // 押し出されたモジュールを元の from スロットへ返却
        }

        to.Set(moduleToMove);
    }

    /// <summary>
    /// to が空の場合のみ from → to へ移動する。
    /// to にモジュールがある場合は何もせず false を返す。
    /// </summary>
    public static bool MoveIfEmpty(ModuleSlot from, ModuleSlot to)
    {
        if (to.HasModule) return false;
        to.Set(from.Remove());
        return true;
    }
}
